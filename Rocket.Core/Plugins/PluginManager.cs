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
using Rocket.API.Plugins;
using Rocket.Compatibility;
using Rocket.Core.Commands;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Plugins.Events;

namespace Rocket.Core.Plugins
{
    public class PluginManager : IPluginManager, ICommandProvider
    {
        private readonly Dictionary<string, Assembly> cachedAssemblies;
        private readonly IDependencyContainer container;

        private readonly IEventManager eventManager;
        private readonly ILogger logger;
        private readonly IDependencyContainer parentContainer;
        private readonly IDependencyResolver resolver;
        private readonly IRuntime runtime;

        private Dictionary<IPlugin, List<ICommand>> commands;
        private Dictionary<string, string> packageAssemblies;
        private string packagesDirectory;
        private Dictionary<string, string> pluginAssemblies;
        private string pluginsDirectory;
       
        public PluginManager(IDependencyContainer dependencyContainer, IDependencyResolver resolver, ILogger logger,
                             IEventManager eventManager, IRuntime runtime)
        {
            this.runtime = runtime;
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

        public virtual void Init()
        {
            var runtime = container.Resolve<IRuntime>();

            pluginsDirectory = Path.Combine(runtime.WorkingDirectory, "Plugins");
            packagesDirectory = Path.Combine(runtime.WorkingDirectory, "Packages");

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

                logger.LogDebug(((AppDomain)sender).FriendlyName + " could not find dependency: " + args.Name + " for: " + sender);
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

            IEnumerable<IPlugin> plugins = container.ResolveAll<IPlugin>();
            foreach (IPlugin plugin in plugins)
            {
                Assembly asm = plugin.GetType().Assembly;
                string pluginDir = plugin.WorkingDirectory;

                foreach (string s in asm.GetManifestResourceNames())
                    using (Stream stream = asm.GetManifestResourceStream(s))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (stream == null)
                                return;

                            stream.CopyTo(ms);
                            byte[] data = ms.ToArray();
                            string fileName = s.Replace(plugin.GetType().Namespace, "");
                            File.WriteAllBytes(Path.Combine(pluginDir, fileName), data);
                        }
                    }

                plugin.Activate();
            }
        }

        public virtual IPlugin GetPlugin(string name)
        {
            IPlugin pl = container.Resolve<IPlugin>(name);
            if (pl != null)
                return pl;

            //name may also be path or file name
            foreach (IPlugin plugin in container.ResolveAll<IPlugin>())
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
                plugin.Activate();
                return plugin.IsAlive;
            }

            if (File.Exists(name))
            {
                Assembly asm = LoadAssembly(name);
                plugin = LoadPluginFromAssembly(asm);
                if (plugin == null)
                    return false;

                if (!plugin.IsAlive)
                    plugin.Activate();
                return true;
            }

            return true;
        }

        public virtual bool UnloadPlugin(string name)
        {
            IPlugin plugin = GetPlugin(name);
            if (plugin == null || !plugin.IsAlive)
                return false;

            plugin.Deactivate();
            return !plugin.IsAlive;
        }

        public IEnumerable<IPlugin> Plugins => container.ResolveAll<IPlugin>();

        public virtual void ExecuteSoftDependCode(string pluginName, Action<IPlugin> action)
        {
            if (!PluginExists(pluginName))
                return;

            try
            {
                action(GetPlugin(pluginName));
            }
            catch (TypeLoadException)
            {
                //ignored
            }
            catch (MissingMethodException)
            {
                //ignored
            }
        }

        public virtual IEnumerator<IPlugin> GetEnumerator() => Plugins.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual void RegisterCommands(IPlugin plugin, object o)
        {
            foreach (MethodInfo method in o.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                CommandAttribute cmdAttr =
                    (CommandAttribute)method.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault();

                if(cmdAttr == null)
                    continue;

                IEnumerable<CommandAliasAttribute> aliasAttrs = method
                                                                .GetCustomAttributes(typeof(CommandAliasAttribute),
                                                                    true)
                                                                .Cast<CommandAliasAttribute>();

                IEnumerable<CommandCallerAttribute> supportedTypeAttrs = method
                                                                         .GetCustomAttributes(
                                                                             typeof(CommandCallerAttribute), true)
                                                                         .Cast<CommandCallerAttribute>();

                CommandAttributeWrapper wrapper = new CommandAttributeWrapper(o, method, cmdAttr,
                    aliasAttrs.Select(c => c.AliasName).ToArray(),
                    supportedTypeAttrs.Select(c => c.SupportedCaller).ToArray());

                if (!commands.ContainsKey(plugin))
                    commands.Add(plugin, new List<ICommand>());

                List<ICommand> cmds = commands[plugin];
                if (cmds.Any(c => (c as CommandAttributeWrapper)?.Method == method))
                    continue;

                cmds.Add(wrapper);
            }
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
            IEnumerable<Type> pluginCommands =
                pluginInstance.FindTypes<ICommand>(false, c => !typeof(ISubCommand).IsAssignableFrom(c) 
                    && c.GetCustomAttributes(typeof(DontAutoRegisterAttribute), true).Length == 0);
            IEnumerable<Type> dependcyRegistrators = pluginInstance.FindTypes<IDependencyRegistrator>(false);

            foreach (Type registrator in dependcyRegistrators)
                ((IDependencyRegistrator)Activator.CreateInstance(registrator)).Register(container, resolver);

            foreach (Type listener in listeners)
            {
                IEventListener instance = (IEventListener)Activator.CreateInstance(listener, new object[0]);
                eventManager.AddEventListener(pluginInstance, instance);
            }

            List<ICommand> cmdInstanceList = this.commands.ContainsKey(pluginInstance)
                ? this.commands[pluginInstance]
                : new List<ICommand>();

            this.commands.Remove(pluginInstance);

            foreach (Type command in pluginCommands)
                if (cmdInstanceList.All(c => c.GetType() != command))
                    cmdInstanceList.Add((ICommand)Activator.CreateInstance(command, new object[0]));

            this.commands.Add(pluginInstance, cmdInstanceList);
            return pluginInstance;
        }

        ~PluginManager()
        {
            IEnumerable<IPlugin> plugins = container.ResolveAll<IPlugin>();
            foreach (IPlugin plugin in plugins) plugin.Deactivate();
        }
    }
}