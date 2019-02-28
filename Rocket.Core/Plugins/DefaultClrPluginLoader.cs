using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;

namespace Rocket.Core.Plugins
{
    public class DefaultClrPluginLoader : ClrPluginLoader
    {
        private Dictionary<string, string> packageAssemblies;
        private Dictionary<string, string> pluginAssemblies;

        public DefaultClrPluginLoader(IDependencyContainer dependencyContainer,
                             IEventBus eventBus,
                             ILogger logger) :
            base(dependencyContainer, eventBus, logger)
        { }

        protected override async Task<IEnumerable<Assembly>> LoadAssembliesAsync()
        {
            IRuntime runtime = Container.Resolve<IRuntime>();
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                assemblies.Add(assembly);

            /*
            var rocketDire = Path.GetDirectoryName(typeof(DllPluginManager).Assembly.Location);
            var rocketAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(rocketDir);
            foreach (var entry in rocketAssemblies)
                Logger.LogDebug("Loaded rocket assembly: " + entry.Key + " -> " + entry.Value);
            */

            var librariesDirectory = Path.Combine(runtime.WorkingDirectory, "Libraries");
            Directory.CreateDirectory(librariesDirectory);
            packageAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(librariesDirectory);
            foreach (var entry in packageAssemblies)
                Logger.LogDebug("Loaded library: " + entry.Key + " -> " + entry.Value);

            var pluginsDirectory = Path.Combine(runtime.WorkingDirectory, "Plugins");
            Directory.CreateDirectory(pluginsDirectory);
            pluginAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(pluginsDirectory);
            foreach (var entry in pluginAssemblies)
                Logger.LogDebug("Loaded plugin: " + entry.Key + " -> " + entry.Value);

            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                var name = ReflectionExtensions.GetVersionIndependentName(args.Name);

                try
                {
                    if (pluginAssemblies.TryGetValue(name, out string pluginFile))
                        return LoadCachedAssembly(pluginFile);

                    if (packageAssemblies.TryGetValue(name, out string packageFile))
                        return LoadCachedAssembly(packageFile);

                    foreach (var asm in assemblies)
                    {
                        var asmName = ReflectionExtensions.GetVersionIndependentName(asm.FullName);
                        if (asmName.Equals(name))
                            return asm;
                    }

                    Logger.LogDebug("Could not find dependency: " + name);
                }
                catch (Exception ex)
                {
                    Logger.LogFatal("Failed to load assembly: " + name, ex);
                }
                return null;
            };

            foreach (string pluginPath in pluginAssemblies.Values)
                try
                {
                    Assembly pluginAssembly = LoadCachedAssembly(pluginPath);
                    assemblies.Add(pluginAssembly);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to load plugin assembly at {pluginPath}", ex);
                }

            return assemblies;
        }

        public override string ServiceName => "DefaultClrPlugins";
    }
}