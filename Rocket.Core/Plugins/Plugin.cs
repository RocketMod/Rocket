using System;
using System.Collections.Generic;
using System.IO;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;
using Rocket.Core.Plugins.Events;

namespace Rocket.Core.Plugins
{
    public abstract class Plugin<TConfig> : Plugin where TConfig : class, new()
    {
        public virtual TConfig ConfigurationInstance { get; set; }

        protected Plugin(IDependencyContainer container) : base(container) { }
        protected Plugin(string name, IDependencyContainer container) : base(name, container) { }

        public override object DefaultConfiguration => new TConfig();

        public override void SaveConfiguration()
        {
            base.SaveConfiguration();
            Configuration.Set(ConfigurationInstance);
            Configuration?.Save();
        }

        public override void LoadConfiguration()
        {
            base.LoadConfiguration();
            if (Configuration == null)
                return;

            ConfigurationInstance = (TConfig)Configuration.Get(typeof(TConfig));
        }
    }

    public abstract class Plugin : IPlugin, ITranslatable, IConfigurable
    {
        // The parent logger is used to log stuff that should not be logged to the plugins own log.
        private readonly ILogger parentLogger;
        protected Plugin(IDependencyContainer container) : this(null, container) { }

        protected Plugin(string name, IDependencyContainer container)
        {
            parentLogger = container.ParentContainer.Resolve<ILogger>();

            string assemblyName = null;
            try
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // Resharper does not know that dynamically byte[] loaded assemblies can return null for Location
                if (GetType().Assembly?.Location != null)
                {
                    assemblyName = Path.GetFileNameWithoutExtension(GetType().Assembly.Location);
                }
            }
            catch
            {

            }

            Name = name ?? assemblyName ?? GetType().Name;
            Name = Name.Replace(" ", "_");

            Container = container;
            container.RegisterSingletonInstance<ILogger>(new ProxyLogger(Container), null, "proxy_logger");
            Container.RegisterSingletonInstance<IPlugin>(this);

            WorkingDirectory = Path.Combine(Path.Combine(Runtime.WorkingDirectory, "Plugins"), Name.Replace("_", " "));

            IRocketSettingsProvider rocketSettings = Container.Resolve<IRocketSettingsProvider>();
            if (rocketSettings.Settings.Logging.EnableSeparatePluginLogs)
                Container.RegisterSingletonType<ILogger, PluginLogger>("plugin_logger");
        }

        public virtual void SaveConfiguration()
        {
            Configuration?.Save();
        }

        protected IEventManager EventManager => Container.Resolve<IEventManager>();
        protected ILogger Logger => Container.Resolve<ILogger>();

        protected IRuntime Runtime => Container.Resolve<IRuntime>();

        protected IHost Host => Container.Resolve<IHost>();
        public IConfiguration Configuration { get; protected set; }

        public virtual object DefaultConfiguration
            => null;

        public IDependencyContainer Container { get; }

        public virtual IPluginManager PluginManager => Container.Resolve<IPluginManager>();

        public string Name { get; }

        public virtual string WorkingDirectory { get; set; }
        public string ConfigurationName => Name;

        public bool Load(bool isReload)
        {
            if (IsAlive)
                return false;

            parentLogger.LogInformation($"Loading {Name}.");

            if (isReload)
            {
                Configuration?.Reload();
                Translations?.Reload();
            }

            if (EventManager != null)
            {
                PluginLoadEvent loadEvent = new PluginLoadEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadEvent);
                if (loadEvent.IsCancelled)
                    return false;
            }

            LoadConfiguration();

            if (DefaultTranslations != null)
            {
                Translations = Container.Resolve<ITranslationCollection>();
                IConfigurationContext context = this.CreateChildConfigurationContext("Translations");
                Translations.Load(context, DefaultTranslations);
            }

            IsAlive = true;
            try
            {
                OnLoad(isReload);
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"Failed to load {Name}: ", ex);
                IsAlive = false;
                return false;
            }

            if (EventManager != null)
            {
                PluginLoadedEvent loadedEvent = new PluginLoadedEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }

            return true;
        }

        public virtual void LoadConfiguration()
        {
            if (DefaultConfiguration != null)
            {
                Configuration = Container.Resolve<IConfiguration>();
                IConfigurationContext context = this.CreateChildConfigurationContext("Configuration");
                Configuration.Scheme = DefaultConfiguration.GetType();
                Configuration.Load(context, DefaultConfiguration);
            }
        }

        public bool Unload()
        {
            if (!IsAlive)
                return false;

            parentLogger.LogInformation($"Unloading {Name}.");

            if (EventManager != null)
            {
                PluginUnloadEvent loadedEvent = new PluginUnloadEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }

            try
            {
                OnUnload();
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"An error occured on unloading {Name}: ", ex);
            }

            IsAlive = false;

            if (EventManager != null)
            {
                PluginUnloadedEvent loadedEvent = new PluginUnloadedEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }

            return true;
        }

        public bool IsAlive { get; internal set; }

        public ITranslationCollection Translations { get; protected set; }
        public virtual Dictionary<string, string> DefaultTranslations => null;

        public void RegisterCommandsFromObject(object o)
        {
            DllPluginManager p = PluginManager as DllPluginManager;
            p?.RegisterCommands(Container, o);
        }

        protected virtual void OnLoad(bool isFromReload) { }
        protected virtual void OnUnload() { }

        public void Subscribe(IEventListener listener)
        {
            EventManager.AddEventListener(this, listener);
        }

        public void Subscribe<T>(EventCallback<T> callback) where T : IEvent
        {
            EventManager.Subscribe(this, callback);
        }

        public void Subscribe(string eventName, EventCallback callback)
        {
            EventManager.Subscribe(this, eventName, callback);
        }

        public void Emit(IEvent @event)
        {
            EventManager.Emit(this, @event);
        }
    }
}