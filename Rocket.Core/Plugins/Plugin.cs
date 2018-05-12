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
    public abstract class Plugin : IPlugin, ITranslatable, IConfigurable
    {
        // The parent logger is used to log stuff that should not be logged to the plugins own log.
        private readonly ILogger parentLogger;
        protected Plugin(IDependencyContainer container) : this(null, container) { }

        protected Plugin(string name, IDependencyContainer container)
        {
            parentLogger = container.ParentContainer.Resolve<ILogger>();
            Name = name ?? GetType().Name;

            Container = container;
            container.RegisterSingletonInstance<ILogger>(new ProxyLogger(Container), null, "proxy_logger");
            Container.RegisterSingletonInstance<IPlugin>(this);

            WorkingDirectory = Path.Combine(Path.Combine(Runtime.WorkingDirectory, "Plugins"), Name);

            IRocketSettingsProvider rocketSettings = Container.Resolve<IRocketSettingsProvider>();
            if (rocketSettings.Settings.Logging.EnableSeparatePluginLogs)
                Container.RegisterSingletonType<ILogger, PluginLogger>("plugin_logger");
        }

        protected IEventManager EventManager => Container.Resolve<IEventManager>();
        protected ILogger Logger => Container.Resolve<ILogger>();

        protected IRuntime Runtime => Container.Resolve<IRuntime>();

        protected IImplementation Implementation => Container.Resolve<IImplementation>();
        public IConfiguration Configuration { get; protected set; }

        public virtual object DefaultConfiguration
            => null;

        public IDependencyContainer Container { get; }

        public virtual IPluginManager PluginManager => Container.Resolve<IPluginManager>("default_plugins");

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

            try
            {
                ObLoad(isReload);
                IsAlive = true;
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"Failed to load {Name}: ", ex);
            }

            return true;
        }

        public bool Unload()
        {
            if (!IsAlive)
                return false;

            parentLogger.LogInformation($"Unloading {Name}.");

            if (EventManager != null)
            {
                PluginDeactivateEvent loadedEvent = new PluginDeactivateEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }

            try
            {
                OnDeactivate();
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"An error occured on unloading {Name}: ", ex);
            }

            IsAlive = false;

            if (EventManager != null)
            {
                PluginDeactivatedEvent loadedEvent = new PluginDeactivatedEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }

            return true;
        }

        public bool IsAlive { get; internal set; }

        public ITranslationCollection Translations { get; protected set; }
        public virtual Dictionary<string, string> DefaultTranslations => null;

        public void ObLoad(bool isReload)
        {
            if (EventManager != null)
            {
                PluginActivateEvent activateEvent = new PluginActivateEvent(PluginManager, this);
                EventManager.Emit(Runtime, activateEvent);
                if (activateEvent.IsCancelled)
                    return;
            }

            if (DefaultConfiguration != null)
            {
                Configuration = Container.Resolve<IConfiguration>();
                IConfigurationContext context = this.CreateChildConfigurationContext("Configuration");
                Configuration.Scheme = DefaultConfiguration.GetType();
                Configuration.Load(context, DefaultConfiguration);
            }

            if (DefaultTranslations != null)
            {
                Translations = Container.Resolve<ITranslationCollection>();
                IConfigurationContext context = this.CreateChildConfigurationContext("Translations");
                Translations.Load(context, DefaultTranslations);
            }

            OnActivate(isReload);
            IsAlive = true;

            if (EventManager != null)
            {
                PluginActivatedEvent activatedEvent = new PluginActivatedEvent(PluginManager, this);
                EventManager.Emit(Runtime, activatedEvent);
            }
        }

        public void RegisterCommandsFromObject(object o)
        {
            PluginManager p = PluginManager as PluginManager;
            p?.RegisterCommands(Container, o);
        }

        protected virtual void OnActivate(bool isFromReload) { }
        protected virtual void OnDeactivate() { }

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