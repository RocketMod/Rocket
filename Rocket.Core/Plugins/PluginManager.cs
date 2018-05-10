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
    public class PluginManager : IPluginManager
    {
        private readonly Dictionary<string, Assembly> cachedAssemblies;
        private readonly IDependencyContainer container;

        private readonly IEventManager eventManager;
        private readonly ILogger logger;
        private readonly IDependencyContainer parentContainer;
        private readonly IDependencyResolver resolver;

        private Dictionary<string, string> packageAssemblies;
        private string packagesDirectory;
        private Dictionary<string, string> pluginAssemblies;
        private string pluginsDirectory;

        public PluginManager(IDependencyContainer dependencyContainer,
                             IDependencyResolver resolver, ILogger logger,
                             IEventManager eventManager)
        {
            this.resolver = resolver;
            this.logger = logger;
            this.eventManager = eventManager;
            parentContainer = dependencyContainer;
            container = dependencyContainer.CreateChildContainer();
            cachedAssemblies = new Dictionary<string, Assembly>();
        }

        public virtual void Init()
        {
            IRuntime runtime = container.Resolve<IRuntime>();

            pluginsDirectory = Path.Combine(runtime.WorkingDirectory, "Plugins");
            packagesDirectory = Path.Combine(runtime.WorkingDirectory, "Packages");

            Directory.CreateDirectory(pluginsDirectory);
            pluginAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(pluginsDirectory);

            Directory.CreateDirectory(packagesDirectory);
            packageAssemblies = ReflectionExtensions.GetAssembliesFromDirectory(packagesDirectory);

            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
            {
                if (pluginAssemblies.TryGetValue(args.Name, out string pluginFile))
                    return LoadAssembly(pluginFile);

                if (packageAssemblies.TryGetValue(args.Name, out string packageFile))
                    return LoadAssembly(packageFile);

                logger.LogDebug(((AppDomain) sender).FriendlyName
                    + " could not find dependency: "
                    + args.Name
                    + " for: "
                    + sender);
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

            PluginManagerInitEvent pluginManagerInitEvent = new PluginManagerInitEvent(this, EventExecutionTargetContext.Sync);
            eventManager.Emit(runtime, pluginManagerInitEvent);

            if (pluginManagerInitEvent.IsCancelled)
                return;

            List<IDependencyContainer> pluginContainers = new List<IDependencyContainer>();
            foreach (Assembly assembly in assemblies)
            {
                LoadPluginFromAssembly(assembly, out IDependencyContainer container);
                if (container != null)
                    pluginContainers.Add(container);
            }

            foreach (IDependencyContainer childContainer in pluginContainers)
            {
                IPlugin plugin = childContainer.Resolve<IPlugin>();
                
                PluginCommandProvider cmdProvider = new PluginCommandProvider(plugin, childContainer);
                parentContainer.RegisterSingletonInstance<ICommandProvider>(cmdProvider, plugin.Name);

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

                plugin.Load(false);
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

        public virtual bool LoadPlugin(string name)
        {
            IPlugin plugin = GetPlugin(name);
            if (plugin != null)
            {
                if (plugin.IsAlive)
                    return false;

                plugin.Load(false);
                return plugin.IsAlive;
            }

            if (File.Exists(name))
            {
                Assembly asm = LoadAssembly(name);
                plugin = LoadPluginFromAssembly(asm, out IDependencyContainer _);
                if (plugin == null)
                    return false;

                if (!plugin.IsAlive)
                    plugin.Load(true);
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

        public virtual void RegisterCommands(IDependencyContainer pluginContainer, object @object)
        {
            foreach (MethodInfo method in @object.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                CommandAttribute cmdAttr =
                    (CommandAttribute) method.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault();

                if (cmdAttr == null)
                    continue;

                IEnumerable<CommandAliasAttribute> aliasAttrs = method
                                                                .GetCustomAttributes(typeof(CommandAliasAttribute),
                                                                    true)
                                                                .Cast<CommandAliasAttribute>();

                IEnumerable<CommandUserAttribute> supportedTypeAttrs = method
                                                                       .GetCustomAttributes(
                                                                           typeof(CommandUserAttribute), true)
                                                                       .Cast<CommandUserAttribute>();

                CommandAttributeWrapper wrapper = new CommandAttributeWrapper(@object, method, cmdAttr,
                    aliasAttrs.Select(c => c.AliasName).ToArray(),
                    supportedTypeAttrs.Select(c => c.UserType).ToArray());

                pluginContainer.RegisterSingletonInstance<ICommand>(wrapper, wrapper.Name);
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

        public virtual IPlugin LoadPluginFromAssembly(Assembly pluginAssembly, out IDependencyContainer childContainer)
        {
            childContainer = null;

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

                if (type.GetCustomAttributes(typeof(DontAutoRegisterAttribute), true).Any())
                    continue;

                if (pluginType == null && typeof(IPlugin) != type && typeof(IPlugin).IsAssignableFrom(type))
                    pluginType = type;
            }

            if (pluginType == null)
                return null;

            childContainer = container.CreateChildContainer();
            IPlugin pluginInstance = (IPlugin) childContainer.Activate(pluginType);
            container.RegisterInstance(pluginInstance, pluginInstance.Name);

            IEnumerable<Type> listeners = pluginInstance.FindTypes<IEventListener>(false);
            IEnumerable<Type> pluginCommands =
                pluginInstance.FindTypes<ICommand>(false, c => !typeof(IChildCommand).IsAssignableFrom(c)
                    && c.GetCustomAttributes(typeof(DontAutoRegisterAttribute), true).Length == 0);
            IEnumerable<Type> dependencyRegistrators = pluginInstance.FindTypes<IDependencyRegistrator>(false);

            foreach (Type registrator in dependencyRegistrators)
                ((IDependencyRegistrator) Activator.CreateInstance(registrator)).Register(container, resolver);

            foreach (Type listener in listeners)
            {
                IEventListener instance = (IEventListener) Activator.CreateInstance(listener, new object[0]);
                eventManager.AddEventListener(pluginInstance, instance);
            }

            foreach (Type command in pluginCommands)
            {
                ICommand cmdInstance = (ICommand) Activator.CreateInstance(command, new object[0]);
                childContainer.RegisterSingletonInstance(cmdInstance, cmdInstance.Name);
            }

            return pluginInstance;
        }

        ~PluginManager()
        {
            IEnumerable<IPlugin> plugins = container.ResolveAll<IPlugin>();
            foreach (IPlugin plugin in plugins) plugin.Unload();
        }
    }
}