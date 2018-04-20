using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using Rocket.Core.Commands;
#if NET35
using Rocket.Compability;
#endif
using Rocket.Core.Extensions;
using Rocket.Core.Plugins.Events;

namespace Rocket.Core.Plugins
{
    public class PluginManager : IPluginManager, ICommandProvider
    {
        private string pluginsDirectory;
        private string packagesDirectory;

        private readonly IEventManager eventManager;
        private readonly IDependencyContainer container;
        private readonly ILogger logger;
        private readonly IRuntime runtime;
        private readonly IDependencyContainer parentContainer;
        private readonly IDependencyResolver resolver;
        private readonly IImplementation implementation;

        private readonly Dictionary<string, Assembly> cachedAssemblies;

        private Dictionary<IPlugin, List<ICommand>> commands;
        private Dictionary<string, string> packageAssemblies;
        private Dictionary<string, string> pluginAssemblies;

        public PluginManager(IDependencyContainer dependencyContainer, IDependencyResolver resolver, ILogger logger,
                             IEventManager eventManager, IRuntime runtime, IImplementation implementation)
        {
            this.runtime = runtime;
            this.implementation = implementation;
            this.resolver = resolver;
            this.logger = logger;
            this.eventManager = eventManager;
            parentContainer = dependencyContainer;
            container = dependencyContainer.CreateChildContainer();
            cachedAssemblies = new Dictionary<string, Assembly>();
        }

        public virtual IEnumerable<ICommand> Commands
        {
            get
            {
                return commands
                       .Where(c => c.Key.IsAlive)
                       .SelectMany(c => c.Value);
            }
        }

        public virtual void RegisterCommands(IPlugin plugin, object o)
        {
            foreach (var method in o.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = (CommandAttribute)method.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault();
                CommandAttributeWrapper wrapper = new CommandAttributeWrapper(o, method, attr);
                if (!commands.ContainsKey(plugin))
                    commands.Add(plugin, new List<ICommand>());

                var cmds = commands[plugin];
                if (cmds.Any(c => (c as CommandAttributeWrapper)?.Method == method))
                    continue;

                cmds.Add(wrapper);
            }
        }

        public virtual void Init()
        {
            pluginsDirectory = Path.Combine(implementation.WorkingDirectory, "Plugins");
            packagesDirectory = Path.Combine(implementation.WorkingDirectory, "Packages");

            commands = new Dictionary<IPlugin, List<ICommand>>();

            Directory.CreateDirectory(pluginsDirectory);
            pluginAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(pluginsDirectory);

            Directory.CreateDirectory(packagesDirectory);
            packageAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(packagesDirectory);

            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                if (pluginAssemblies.TryGetValue(args.Name, out string pluginFile))
                    return LoadAssembly(pluginFile);

                if (pluginAssemblies.TryGetValue(args.Name, out string packageFile))
                    return LoadAssembly(packageFile);

                logger.LogDebug(((AppDomain)sender).FriendlyName + " could not find dependency: " + args.Name);
                return null;
            };

            List<Assembly> assemblies = new List<Assembly>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                assemblies.Add(assembly);

            foreach (string pluginPath in pluginAssemblies.Values)
                try
                {
                    Assembly pluginAssembly = LoadAssembly(pluginPath);
                    assemblies.Add(pluginAssembly);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to load plugin assembly at {pluginPath}", ex);
                }

            foreach (Assembly assembly in assemblies)
                LoadPluginFromAssembly(assembly);

            PluginManagerInitEvent @event = new PluginManagerInitEvent(this, EventExecutionTargetContext.Sync);
            eventManager.Emit(runtime, @event);

            if (@event.IsCancelled)
                return;

            container.TryGetAll(out IEnumerable<IPlugin> plugins);
            foreach (IPlugin plugin in plugins)
            {
                var asm = plugin.GetType().Assembly;
                var pluginDir = plugin.WorkingDirectory;

                foreach (var s in asm.GetManifestResourceNames())
                    using (Stream stream = asm.GetManifestResourceStream(s))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (stream == null)
                                return;

                            stream.CopyTo(ms);
                            byte[] data = ms.ToArray();
                            var fileName = s.Replace(plugin.GetType().Namespace, "");
                            File.WriteAllBytes(Path.Combine(pluginDir, fileName), data);
                        }
                    }

                plugin.Load();
            }
        }

        public virtual IPlugin GetPlugin(string name)
        {
            IPlugin pl = container.Get<IPlugin>(name);
            if (pl != null)
                return pl;

            //name may also be path or file name
            foreach (IPlugin plugin in container.GetAll<IPlugin>())
            {
                Type type = plugin.GetType();
                string location = GetAssemblyLocationSafe(type.Assembly);
                if (string.IsNullOrEmpty(location))
                    continue;

                if (location.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileNameWithoutExtension(location).Equals(name, StringComparison.OrdinalIgnoreCase))
                    return plugin;
            }

            return null;
        }

        public virtual bool PluginExists(string name) => container.IsRegistered<IPlugin>(name);

        public virtual bool LoadPlugin(string name)
        {
            IPlugin plugin = GetPlugin(name);
            if (plugin != null && !plugin.IsAlive)
            {
                plugin.Load();
                return plugin.IsAlive;
            }

            if (File.Exists(name))
            {
                Assembly asm = LoadAssembly(name);
                plugin = LoadPluginFromAssembly(asm);
                if (plugin == null)
                    return false;

                if (!plugin.IsAlive)
                    plugin.Load();
                return true;
            }

            return true;
        }

        public virtual bool UnloadPlugin(string name)
        {
            IPlugin plugin = GetPlugin(name);
            if (plugin == null || !plugin.IsAlive)
                return false;

            plugin.Unload();
            return !plugin.IsAlive;
        }

        public IEnumerable<IPlugin> Plugins
        {
            get
            {
                container.TryGetAll<IPlugin>(out var plugins);
                return plugins;
            }
        }

        public virtual bool ExecutePluginDependendCode(string pluginName, Action<IPlugin> action)
        {
            if (PluginExists(pluginName))
            {
                action(GetPlugin(pluginName));
                return true;
            }

            return false;
        }

        protected virtual Assembly LoadAssembly(string path)
        {
            path = path.Trim();

            if (cachedAssemblies.ContainsKey(path))
                return cachedAssemblies[path];

            Assembly asm = Assembly.LoadFile(path);
            cachedAssemblies.Add(path, asm);
            return asm;
        }

        private string GetAssemblyLocationSafe(Assembly asm)
        {
            try
            {
                string location = asm.Location;
                return string.IsNullOrEmpty(location) ? null : location;
            }
            catch
            {
                return null;
            }
        }

        public virtual IPlugin LoadPluginFromAssembly(Assembly pluginAssembly)
        {
            string loc = GetAssemblyLocationSafe(pluginAssembly);

            if (!string.IsNullOrEmpty(loc)
                && !cachedAssemblies.ContainsKey(loc))
                cachedAssemblies.Add(loc, pluginAssembly);

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

            foreach (Type type in types.Where(t => t != null))
            {
                if (type.IsAbstract || type.IsInterface) continue;

                if (pluginType == null && typeof(IPlugin) != type && typeof(IPlugin).IsAssignableFrom(type))
                    pluginType = type;
            }

            if (pluginType == null)
                return null;

            IPlugin pluginInstance = (IPlugin)parentContainer.Activate(pluginType);
            container.RegisterInstance(pluginInstance, pluginInstance.Name);

            IEnumerable<Type> listeners = pluginInstance.FindTypes<IEventListener>(false);
            IEnumerable<Type> commands = pluginInstance.FindTypes<ICommand>(false, c => !typeof(ISubCommand).IsAssignableFrom(c));
            IEnumerable<Type> dependcyRegistrators = pluginInstance.FindTypes<IDependencyRegistrator>(false);

            foreach (Type registrator in dependcyRegistrators) ((IDependencyRegistrator)Activator.CreateInstance(registrator)).Register(container, resolver);

            foreach (Type listener in listeners)
            {
                IEventListener instance = (IEventListener)Activator.CreateInstance(listener, new object[0]);
                eventManager.AddEventListener(pluginInstance, instance);
            }

            List<ICommand> cmdInstanceList = this.commands.ContainsKey(pluginInstance)
                ? this.commands[pluginInstance]
                : new List<ICommand>();

            this.commands.Remove(pluginInstance);

            foreach (Type command in commands)
            {
                if (cmdInstanceList.All(c => c.GetType() != command))
                    cmdInstanceList.Add((ICommand)Activator.CreateInstance(command, new object[0]));
            }

            this.commands.Add(pluginInstance, cmdInstanceList);
            return pluginInstance;
        }

        public virtual IEnumerator<IPlugin> GetEnumerator()
        {
            return Plugins.GetEnumerator();
        }

        ~PluginManager()
        {
            container.TryGetAll(out IEnumerable<IPlugin> plugins);
            foreach (IPlugin plugin in plugins) plugin.Unload();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}