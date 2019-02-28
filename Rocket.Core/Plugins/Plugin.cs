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
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rocket.Core.Plugins
{
    public abstract class Plugin<TConfig> : Plugin where TConfig : class, new()
    {
        public virtual TConfig ConfigurationInstance { get; set; }

        protected Plugin(IDependencyContainer container) : base(container) { }
        protected Plugin(string name, IDependencyContainer container) : base(name, container) { }

        public override object DefaultConfiguration => new TConfig();

        public override async Task SaveConfiguration()
        {
            await base.SaveConfiguration();
            if (Configuration != null)
            {
                Configuration.Set(ConfigurationInstance);
                await Configuration.SaveAsync();
            }
        }

        public override async Task LoadConfiguration()
        {
            await base.LoadConfiguration();
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
            container.RegisterSingletonInstance<ILogger>(new LoggerProxy(Container), null, "proxy_logger");
            Container.RegisterSingletonInstance<IPlugin>(this);

            WorkingDirectory = Path.Combine(Path.Combine(Runtime.WorkingDirectory, "Plugins"), Name.Replace("_", " "));

            IRocketSettingsProvider rocketSettings = Container.Resolve<IRocketSettingsProvider>();
            if (rocketSettings.Settings.Logging.EnableSeparatePluginLogs)
                Container.RegisterSingletonType<ILogger, PluginLogger>("plugin_logger");
        }

        public virtual async Task SaveConfiguration()
        {
            if (Configuration != null)
                await Configuration.SaveAsync();
        }

        protected IEventBus EventBus => Container.Resolve<IEventBus>();
        protected ILogger Logger => Container.Resolve<ILogger>();

        protected IRuntime Runtime => Container.Resolve<IRuntime>();

        protected IHost Host => Container.Resolve<IHost>();
        public IConfiguration Configuration { get; protected set; }

        public virtual object DefaultConfiguration
            => null;

        public IDependencyContainer Container { get; }

        public virtual IPluginLoader PluginLoader => Container.Resolve<IPluginLoader>();

        public string Name { get; }

        public virtual string WorkingDirectory { get; set; }
        public string ConfigurationName => Name;

        public async Task<bool> ActivateAsync(bool isReload)
        {
            if (IsAlive)
                return false;

            parentLogger.LogInformation($"Loading {Name}.");

            if (isReload)
            {
                if (Configuration != null)
                    await Configuration.ReloadAsync();

                if (Translations != null)
                    await Translations.ReloadAsync();
            }

            if (EventBus != null)
            {
                PluginActivateEvent activateEvent = new PluginActivateEvent(PluginLoader, this);
                EventBus.Emit(Runtime, activateEvent);
                if (activateEvent.IsCancelled)
                    return false;
            }

            await LoadConfiguration();

            if (DefaultTranslations != null)
            {
                Translations = Container.Resolve<ITranslationCollection>();
                IConfigurationContext context = this.CreateChildConfigurationContext("Translations");
                await Translations.LoadAsync(context, DefaultTranslations);
            }

            IsAlive = true;
            try
            {
                await OnActivate(isReload);
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"Failed to load {Name}: ", ex);
                IsAlive = false;
                return false;
            }

            if (EventBus != null)
            {
                PluginActivatedEvent loadedEvent = new PluginActivatedEvent(PluginLoader, this);
                EventBus.Emit(Runtime, loadedEvent);
            }

            return true;
        }

        public virtual async Task LoadConfiguration()
        {
            if (DefaultConfiguration != null)
            {
                Configuration = Container.Resolve<IConfiguration>();
                IConfigurationContext context = this.CreateChildConfigurationContext("Configuration");
                Configuration.Scheme = DefaultConfiguration.GetType();
                await Configuration.LoadAsync(context, DefaultConfiguration);
            }
        }

        public async Task<bool> DeactivateAsync()
        {
            if (!IsAlive)
                return false;

            parentLogger.LogInformation($"Unloading {Name}.");

            if (EventBus != null)
            {
                PluginDeactivateEvent loadedEvent = new PluginDeactivateEvent(PluginLoader, this);
                EventBus.Emit(Runtime, loadedEvent);
            }

            try
            {
                await OnDeactivate();
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"An error occured on unloading {Name}: ", ex);
            }

            IsAlive = false;

            if (EventBus != null)
            {
                PluginDeactivatedEvent loadedEvent = new PluginDeactivatedEvent(PluginLoader, this);
                EventBus.Emit(Runtime, loadedEvent);
            }

            return true;
        }

        public bool IsAlive { get; internal set; }

        public ITranslationCollection Translations { get; protected set; }
        public virtual Dictionary<string, string> DefaultTranslations => null;

        public void RegisterCommandsFromObject(object o)
        {
            DefaultClrPluginLoader p = PluginLoader as DefaultClrPluginLoader;
            p?.RegisterCommands(Container, o);
        }

        protected virtual async Task OnActivate(bool isFromReload) { }
        protected virtual async Task OnDeactivate() { }

        public void Subscribe(IEventListener listener)
        {
            EventBus.AddEventListener(this, listener);
        }

        public void Subscribe<T>(EventCallback<T> callback) where T : IEvent
        {
            EventBus.Subscribe(this, callback);
        }

        public void Subscribe(string eventName, EventCallback callback)
        {
            EventBus.Subscribe(this, eventName, callback);
        }

        public void Emit(IEvent @event)
        {
            EventBus.Emit(this, @event);
        }
    }
}