using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using Rocket.API.Reflection;

namespace Rocket.Core.Plugins {
    public class PluginManager : IPluginManager, ICommandProvider {
        private static readonly string pluginsDirectory = "./Plugins/";

        private static readonly string packagesDirectory = "./Packages/";

        private readonly IEventManager eventManager;
        private readonly IDependencyContainer container;
        private readonly ILogger logger;

        private readonly IDependencyContainer parentContainer;

        private Dictionary<IPlugin, List<ICommand>> commands;
        private Dictionary<string, string> packageAssemblies;
        private Dictionary<string, string> pluginAssemblies;

        public PluginManager(IDependencyContainer dependencyContainer, ILogger logger, IEventManager eventManager) {
            parentContainer = dependencyContainer;
            this.logger = logger;
            this.eventManager = eventManager;
            container = dependencyContainer.CreateChildContainer();
        }

        public IEnumerable<ICommand> Commands => commands
            .Where(c => c.Key.IsAlive)
            .SelectMany(c => c.Value);

        public void Init() {
            commands = new Dictionary<IPlugin, List<ICommand>>();

            Directory.CreateDirectory(pluginsDirectory);
            pluginAssemblies = getAssembliesFromDirectory(pluginsDirectory);

            Directory.CreateDirectory(packagesDirectory);
            packageAssemblies = getAssembliesFromDirectory(packagesDirectory);

            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args) {
                if (pluginAssemblies.TryGetValue(args.Name, out string pluginFile))
                    return Assembly.Load(File.ReadAllBytes(pluginFile));

                if (pluginAssemblies.TryGetValue(args.Name, out string packageFile))
                    return Assembly.Load(File.ReadAllBytes(packageFile));

                logger.Error("Could not find dependency: " + args.Name);
                return null;
            };

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) loadPluginFromAssembly(assembly);

            foreach (string pluginPath in pluginAssemblies.Values)
                try {
                    Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);
                    loadPluginFromAssembly(pluginAssembly);
                }
                catch (Exception ex) {
                    logger.Error($"Failed to load plugin assembly at {pluginPath}", ex);
                }

            container.TryGetAll(out IEnumerable<IPlugin> plugins);
            foreach (IPlugin plugin in plugins) plugin.Load();
        }

        public IPlugin GetPlugin(string name) {
            return container.Get<IPlugin>(name);
        }

        public bool PluginExists(string name) {
            return container.IsRegistered<IPlugin>(name);
        }

        public bool LoadPlugin(string name) {
            throw new NotImplementedException();
        }

        public bool UnloadPlugin(string name) {
            throw new NotImplementedException();
        }

        public bool ExecutePluginDependendCode(string pluginName, Action<IPlugin> action) {
            if (PluginExists(pluginName)) {
                action(GetPlugin(pluginName));
                return true;
            }

            return false;
        }

        private Dictionary<string, string> getAssembliesFromDirectory(string directory, string extension = "*.dll") {
            Dictionary<string, string> l = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries =
                new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
                try {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    l.Add(name.FullName, library.FullName);
                }
                catch { }

            return l;
        }

        private void loadPluginFromAssembly(Assembly pluginAssembly) {
            Type[] types;
            try {
                types = pluginAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e) {
                types = e.Types;
            }

            Type pluginType = null;

            foreach (Type type in types.Where(t => t != null)) {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                if (pluginType == null && typeof(IPlugin) != type && typeof(IPlugin).IsAssignableFrom(type))
                    pluginType = type;
            }

            if (pluginType == null)
                return;

            IPlugin pluginInstance = (IPlugin) parentContainer.Activate(pluginType);
            container.RegisterInstance(pluginInstance, pluginInstance.Name);


            List<Type> listeners = pluginInstance.FindTypes<IEventListener>();
            List<Type> commands = pluginInstance.FindTypes<ICommand>();

            foreach (Type listener in listeners) {
                IEventListener instance = (IEventListener) Activator.CreateInstance(listener, new object[0]);
                eventManager.AddEventListener(pluginInstance, instance);
            }

            List<ICommand> cmdInstanceList = this.commands.ContainsKey(pluginInstance)
                ? this.commands[pluginInstance]
                : new List<ICommand>();

            this.commands.Remove(pluginInstance);

            foreach (Type command in commands)
                cmdInstanceList.Add((ICommand) Activator.CreateInstance(command, new object[0]));

            this.commands.Add(pluginInstance, cmdInstanceList);
        }

        ~PluginManager() {
            container.TryGetAll(out IEnumerable<IPlugin> plugins);
            foreach (IPlugin plugin in plugins) plugin.Unload();
        }
    }
}