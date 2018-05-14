using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;

namespace Rocket.Core.Plugins
{
    public class PluginManager : CLRPluginManager
    {
        private Dictionary<string, string> packageAssemblies;
        private string packagesDirectory;
        private Dictionary<string, string> pluginAssemblies;
        private string pluginsDirectory;

        public PluginManager(IDependencyContainer dependencyContainer, 
                             IEventManager eventManager,
                             ILogger logger) : 
            base(dependencyContainer, eventManager, logger) { }

        protected override IEnumerable<Assembly> LoadAssemblies()
        {
            IRuntime runtime = Container.Resolve<IRuntime>();

            pluginsDirectory = Path.Combine(runtime.WorkingDirectory, "Plugins");
            packagesDirectory = Path.Combine(runtime.WorkingDirectory, "Packages");

            Directory.CreateDirectory(pluginsDirectory);
            pluginAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(pluginsDirectory);

            Directory.CreateDirectory(packagesDirectory);
            packageAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(packagesDirectory);

            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                if (pluginAssemblies.TryGetValue(args.Name, out string pluginFile))
                    return LoadCachedAssembly(pluginFile);

                if (packageAssemblies.TryGetValue(args.Name, out string packageFile))
                    return LoadCachedAssembly(packageFile);

                Logger.LogDebug(((AppDomain)sender).FriendlyName
                    + " could not find dependency: "
                    + args.Name
                    + " for: "
                    + sender);
                return null;
            };

            List<Assembly> assemblies = new List<Assembly>();
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

        public override string ServiceName => "DllPlugins";
    }
}