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
using Rocket.Core.Events.Plugins;
using Rocket.Core.I18N;

namespace Rocket.Core.Plugins
{
    public abstract class Plugin : IPlugin, ITranslatable, IConfigurable
    {
        public class CapabilityOptions
        {
            public const string NoConfig = "NoConfig";
            public const string NoTranslations = "NoTranslations";

            public const string CustomConfig = "CustomConfig";
            public const string CustomTranslations = "CustomTranslations";
        }

        public IConfiguration Configuration { get; protected set; }
        public virtual object DefaultConfiguration 
            => null;

        public ITranslations Translations { get; protected set; }
        public Dictionary<string, string> DefaultTranslations 
            => null;
        
        public abstract IEnumerable<string> Capabilities { get; }

        public string Name { get; }

        public virtual string WorkingDirectory { get; set; }

        protected Plugin(IDependencyContainer container) : this(null, container) { }

        protected Plugin(string name, IDependencyContainer container)
        {
            Name = name ?? GetType().Name;
            Container = container.CreateChildContainer();
        }

        public IDependencyContainer Container { get; }
        public IEventManager EventManager => Container.Get<IEventManager>();
        public ILogger Logger => Container.Get<ILogger>();


        public IPluginManager PluginManager => Container.Get<IPluginManager>();

        public IRuntime Runtime => Container.Get<IRuntime>();

        public IImplementation Implementation => Container.Get<IImplementation>();

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

            if (!Capabilities.Any(c => c.Equals(CapabilityOptions.CustomConfig, StringComparison.OrdinalIgnoreCase))
                && !Capabilities.Any(c => c.Equals(CapabilityOptions.NoConfig, StringComparison.OrdinalIgnoreCase))
                && DefaultConfiguration != null)
            {
                Configuration = Container.Get<IConfiguration>();
                Configuration.Load(new EnvironmentContext()
                {
                    WorkingDirectory = WorkingDirectory,
                    Name = Name + ".Configuration"
                }, DefaultConfiguration);
            }

            if (!Capabilities.Any(c
                    => c.Equals(CapabilityOptions.CustomTranslations, StringComparison.OrdinalIgnoreCase))
                && !Capabilities.Any(
                    c => c.Equals(CapabilityOptions.NoTranslations, StringComparison.OrdinalIgnoreCase)) 
                && DefaultTranslations != null)
            {
                Translations = Container.Get<ITranslations>();
                Translations.Load(new EnvironmentContext()
                {
                    WorkingDirectory = WorkingDirectory,
                    Name = Name + ".Translations"
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