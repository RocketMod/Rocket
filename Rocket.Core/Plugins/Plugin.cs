using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using Rocket.Core.I18N;
using Rocket.Core.Plugins.Events;

namespace Rocket.Core.Plugins
{
    public abstract class Plugin : IPlugin, ITranslatable, IConfigurable
    {
        public IConfiguration Configuration { get; protected set; }
        public virtual object DefaultConfiguration 
            => null;

        public ITranslationLocator Translations { get; protected set; }
        public Dictionary<string, string> DefaultTranslations => null;

        public string Name { get; }

        public virtual string WorkingDirectory { get; set; }

        protected Plugin(IDependencyContainer container) : this(null, container) { }

        protected Plugin(string name, IDependencyContainer container)
        {
            Name = name ?? GetType().Name;
            Container = container.CreateChildContainer();
        }

        protected IDependencyContainer Container { get; }
        protected IEventManager EventManager => Container.Get<IEventManager>();
        protected ILogger Logger => Container.Get<ILogger>();


        protected IPluginManager PluginManager => Container.Get<IPluginManager>();

        protected IRuntime Runtime => Container.Get<IRuntime>();

        protected IImplementation Implementation => Container.Get<IImplementation>();

        public void Load()
        {
            WorkingDirectory = Path.Combine(Path.Combine(Implementation.WorkingDirectory, "Plugins"), Name);

            if (EventManager != null)
            {
                PluginLoadEvent loadEvent = new PluginLoadEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadEvent);
                if (loadEvent.IsCancelled)
                    return;
            }

            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);

            if (DefaultConfiguration != null)
            {
                Configuration = Container.Get<IConfiguration>();
                Configuration.Load(new ConfigurationContext
                {
                    WorkingDirectory = WorkingDirectory,
                    ConfigurationName = Name + ".Configuration"
                }, DefaultConfiguration);
            }

            if (DefaultTranslations != null)
            {
                Translations = Container.Get<ITranslationLocator>();
                Translations.Load(new ConfigurationContext
                {
                    WorkingDirectory = WorkingDirectory,
                    ConfigurationName = Name + ".Translations"
                }, DefaultTranslations);
            }

            OnLoad();
            IsAlive = true;

            if (EventManager != null)
            {
                PluginLoadedEvent loadedEvent = new PluginLoadedEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }
        }

        public void Unload()
        {
            if (EventManager != null)
            {
                PluginUnloadEvent loadedEvent = new PluginUnloadEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }

            OnUnload();
            IsAlive = false;

            if (EventManager != null)
            {
                PluginUnloadedEvent loadedEvent = new PluginUnloadedEvent(PluginManager, this);
                EventManager.Emit(Runtime, loadedEvent);
            }
        }

        public bool IsAlive { get; internal set; }

        protected virtual void OnLoad() { }

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