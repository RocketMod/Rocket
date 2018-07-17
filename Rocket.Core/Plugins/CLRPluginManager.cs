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
using Rocket.Core.Commands;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Eventing;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Plugins.Events;
#if NET35
using Theraot.Core;
#endif

namespace Rocket.Core.Plugins
{
    public abstract class CLRPluginManager : IPluginManager, IDisposable
    {
        protected IDependencyContainer Container { get; }
        protected IEventBus EventBus { get; }
        protected ILogger Logger { get; }
        protected IDependencyContainer ParentContainer { get; }

        private readonly Dictionary<string, Assembly> cachedAssemblies;
        private IEnumerable<Assembly> assemblies;

        protected CLRPluginManager(IDependencyContainer dependencyContainer,
                                   IEventBus eventBus, ILogger logger)
        {
            EventBus = eventBus;
            Logger = logger;
            ParentContainer = dependencyContainer;
            Container = dependencyContainer.CreateChildContainer();
            cachedAssemblies = new Dictionary<string, Assembly>();
        }

        public void Dispose()
        {
            IEnumerable<IPlugin> plugins = Container.ResolveAll<IPlugin>();

            foreach (IPlugin plugin in plugins)
                plugin.Unload();
        }

        public virtual void Init()
        {
            Logger.LogDebug($"[{GetType().Name}] Initializing CLR plugins.");

            assemblies = LoadAssemblies();

            IRuntime runtime = Container.Resolve<IRuntime>();

            PluginManagerInitEvent pluginManagerInitEvent =
                new PluginManagerInitEvent(this, EventExecutionTargetContext.Sync);
            EventBus.Emit(runtime, pluginManagerInitEvent);

            if (pluginManagerInitEvent.IsCancelled)
            {
                Logger.LogDebug($"[{GetType().Name}] Loading of plugins was cancelled.");
                return;
            }


            List<IDependencyContainer> pluginContainers = new List<IDependencyContainer>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Logger.LogDebug($"[{GetType().Name}] Loading from assembly: " + assembly.GetName().Name);
                    LoadPluginFromAssembly(assembly, out IDependencyContainer container);

                    if (container != null)
                    {
                        pluginContainers.Add(container);
                        Logger.LogDebug($"[{GetType().Name}] Plugin found in: " + assembly.GetName().Name);
                    }
                    else
                    {
                        Logger.LogDebug($"[{GetType().Name}] No plugins found in: " + assembly.GetName().Name);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"[{GetType().Name}] Failed to load from assembly: " + assembly.GetName().Name, ex);
                }
            }

            foreach (IDependencyContainer childContainer in pluginContainers)
                RegisterAndLoadPluginFromContainer(childContainer);
        }

        protected bool RegisterAndLoadPluginFromContainer(IDependencyContainer container)
        {
            IPlugin plugin = container.Resolve<IPlugin>();

            Logger.LogDebug($"[{GetType().Name}] Trying to load plugin: " + plugin.Name);

            PluginCommandProvider cmdProvider = new PluginCommandProvider(plugin, container);
            ParentContainer.RegisterSingletonInstance<ICommandProvider>(cmdProvider, plugin.Name);

            var asm = plugin.GetType().Assembly;
            string pluginDir = plugin.WorkingDirectory;
            //if (!Directory.Exists(pluginDir))
            //    Directory.CreateDirectory(pluginDir);

            foreach (string s in asm.GetManifestResourceNames())
                using (Stream stream = asm.GetManifestResourceStream(s))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        if (stream != null)
                        {
                            stream.CopyTo(ms);
                            byte[] data = ms.ToArray();
                            string fileName = s.Replace(plugin.GetType().Namespace, "");
                            File.WriteAllBytes(Path.Combine(pluginDir, fileName), data);
                        }
                    }
                }

            bool success = plugin.Load(false);
            if (!success)
                return false;

            IEnumerable<Type> listeners = plugin.FindTypes<IAutoRegisteredEventListener>(false);
            foreach (Type listener in listeners)
            {
                IAutoRegisteredEventListener instance = (IAutoRegisteredEventListener)container.Activate(listener);
                EventBus.AddEventListener(plugin, instance);
            }

            return true;
        }

        public virtual IPlugin GetPlugin(string name)
        {
            IPlugin pl = Container.Resolve<IPlugin>(name);
            if (pl != null)
                return pl;

            //name may also be path or file name
            foreach (IPlugin plugin in Container.ResolveAll<IPlugin>())
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

        public virtual bool PluginExists(string name) => Container.IsRegistered<IPlugin>(name);

        public IEnumerable<IPlugin> Plugins => Container.ResolveAll<IPlugin>();

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

        public abstract string ServiceName { get; }

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
                Assembly asm = LoadCachedAssembly(name);
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
                    (CommandAttribute)method.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault();

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

        protected abstract IEnumerable<Assembly> LoadAssemblies();

        protected virtual Assembly LoadCachedAssembly(string path)
        {
            path = path.Trim();

            if (cachedAssemblies.ContainsKey(path))
                return cachedAssemblies[path];


            var data = File.ReadAllBytes(path);

            Assembly asm = Assembly.Load(data);
            cachedAssemblies.Add(path, asm);
            return asm;
        }

        protected virtual IPlugin LoadPluginFromAssembly(Assembly pluginAssembly, out IDependencyContainer childContainer)
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

            childContainer = Container.CreateChildContainer();
            childContainer.RegisterInstance<IPluginManager>(this);

            IPlugin pluginInstance = (IPlugin)childContainer.Activate(pluginType);
            if (pluginInstance == null)
            {
                throw new Exception("Failed to activate: " + pluginType.FullName + ". Is your plugin constructor public?");
            }

            Container.RegisterInstance(pluginInstance, pluginInstance.Name);

            childContainer.RegisterInstance(pluginInstance);

            IEnumerable<Type> pluginCommands =
                pluginInstance.FindTypes<ICommand>(false, c => !typeof(IChildCommand).IsAssignableFrom(c)
                    && c.GetCustomAttributes(typeof(DontAutoRegisterAttribute), true).Length == 0);
            IEnumerable<Type> dependencyRegistrators = pluginInstance.FindTypes<IDependencyRegistrator>(false);

            foreach (Type registrator in dependencyRegistrators)
                ((IDependencyRegistrator)Activator.CreateInstance(registrator)).Register(Container, Container);

            foreach (Type command in pluginCommands)
            {
                try
                {
                    ICommand cmdInstance = (ICommand)childContainer.Activate(command);
                    if (cmdInstance != null)
                        childContainer.RegisterSingletonInstance(cmdInstance, cmdInstance.Name);
                }
                catch (Exception ex)
                {
                    Logger.LogError(null, ex);
                }
            }

            return pluginInstance;
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
    }
}