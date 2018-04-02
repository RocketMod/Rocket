using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.API.Eventing;
using Rocket.Core.Eventing;

namespace Rocket.Core.Plugins
{
    public class PluginManager : IPluginManager
    {
        private static readonly string pluginsDirectory = "./Plugins/";
        private Dictionary<string,string> pluginAssemblies;

        private static readonly string packagesDirectory = "./Packages/";
        private Dictionary<string, string> packageAssemblies;

        private Dictionary<string, string> getAssembliesFromDirectory(string directory, string extension = "*.dll")
        {
            Dictionary<string, string> l = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries = new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    l.Add(name.FullName, library.FullName);
                }
                catch { }
            }
            return l;
        }

        private readonly IDependencyContainer parentContainer;
        private readonly IDependencyContainer container;
        private ILogger logger;

        private readonly IEventManager _eventManager;

        public PluginManager(IDependencyContainer dependencyContainer, ILogger logger, IEventManager eventManager)
        {
            this.parentContainer = dependencyContainer;
            this.logger = logger;
            _eventManager = eventManager;
            container = dependencyContainer.CreateChildContainer();
        }

        public void Init()
        {
            Directory.CreateDirectory(pluginsDirectory);
            pluginAssemblies = getAssembliesFromDirectory(pluginsDirectory);

            Directory.CreateDirectory(packagesDirectory);
            packageAssemblies = getAssembliesFromDirectory(packagesDirectory);

            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                if (pluginAssemblies.TryGetValue(args.Name, out string pluginFile))
                {
                    return Assembly.Load(File.ReadAllBytes(pluginFile));
                }
                else
                if (pluginAssemblies.TryGetValue(args.Name, out string packageFile))
                {
                    return Assembly.Load(File.ReadAllBytes(packageFile));
                }
                else
                {
                    logger.Error("Could not find dependency: " + args.Name);
                }
                return null;
            };

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                loadPluginFromAssembly(assembly);
            }

            foreach (string pluginPath in pluginAssemblies.Values)
            {
                try
                {
                    Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);
                    loadPluginFromAssembly(pluginAssembly);
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to load plugin assembly at {pluginPath}", ex);
                }
            }

            container.TryGetAll<IPlugin>(out IEnumerable<IPlugin> plugins);
            foreach (IPlugin plugin in plugins)
            {
                plugin.Load();
            }
        }

        private void loadPluginFromAssembly(Assembly pluginAssembly)
        {
            Type[] types;
            try
            {
                types = pluginAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            Type pluginType = null;
            List<Type> listeners = new List<Type>();

            foreach (Type type in types.Where(t => t != null))
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                if (typeof(IAutoRegisterEventListener).IsAssignableFrom(type))
                {
                    listeners.Add(type);
                }

                if (pluginType == null && typeof(IPlugin) != type && typeof(IPlugin).IsAssignableFrom(type))
                {
                    pluginType = type;
                }
            }

            if (pluginType == null)
                return;

            IPlugin pluginInstance = (IPlugin)parentContainer.Activate(pluginType);
            container.RegisterInstance<IPlugin>(pluginInstance, pluginInstance.Name);

            foreach (var listener in listeners)
            {
                var instance = (IEventListener) Activator.CreateInstance(listener, new object[0]);
                _eventManager.AddEventListener(pluginInstance, instance);
            }
        }

        ~PluginManager()
        {
            container.TryGetAll<IPlugin>(out IEnumerable<IPlugin> plugins);
            foreach (IPlugin plugin in plugins)
            {
                plugin.Unload();
            }
        }

        public bool HandleCommand(string commandLine)
        {
            return true;    
        }

        public IPlugin GetPlugin(string name)
        {
            return container.Get<IPlugin>(name);
        }

        public bool PluginExists(string name)
        {
            return container.IsRegistered<IPlugin>(name);
        }

        public bool LoadPlugin(string name)
        {
            throw new NotImplementedException();
        }

        public bool UnloadPlugin(string name)
        {
            throw new NotImplementedException();
        }

        public bool ExecutePluginDependendCode(string pluginName, Action<IPlugin> action)
        {
            if (PluginExists(pluginName))
            {
                action(GetPlugin(pluginName));
                return true;
            }
            return false;
        }
    }
}