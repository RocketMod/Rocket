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
        public virtual object DefaultConfiguration => throw new NotImplementedException("Default configuration has not been set up.");

        public ITranslations Translations { get; protected set; }
        public Dictionary<string, string> DefaultTranslations => throw new NotImplementedException("Default translations have not been set up.");

        protected Plugin(IDependencyContainer container) : this(null, container) { }

        protected Plugin(string name, IDependencyContainer container)
        {
            Name = name ?? GetType().Name;

            Container = container.CreateChildContainer();
            EventManager = Container.Get<IEventManager>();
            Logger = Container.Get<ILogger>();
        }

        public IDependencyContainer Container { get; }
        public IEventManager EventManager { get; }
        public ILogger Logger { get; }

        public abstract IEnumerable<string> Capabilities { get; }

        public string Name { get; }

        public void Load()
        {
            IEventManager eventManager = Container.Get<IEventManager>();
            IRuntime runtime = Container.Get<IRuntime>();

            if (eventManager != null)
            {
                PluginLoadEvent loadEvent = new PluginLoadEvent(this);
                eventManager.Emit(runtime, loadEvent);
                if (loadEvent.IsCancelled)
                    return;
            }

            if (!Capabilities.Any(c => c.Equals(CapabilityOptions.CustomConfig, StringComparison.OrdinalIgnoreCase))
                && !Capabilities.Any(c => c.Equals(CapabilityOptions.NoConfig, StringComparison.OrdinalIgnoreCase)))
            {
                Configuration = Container.Get<IConfiguration>();
                Configuration.Load(new EnvironmentContext()
                {
                    WorkingDirectory = Path.Combine("Plugins", ""),
                    Name = Name + ".Configuration"
                }, DefaultConfiguration);
            }

            if (!Capabilities.Any(c
                    => c.Equals(CapabilityOptions.CustomTranslations, StringComparison.OrdinalIgnoreCase))
                && !Capabilities.Any(
                    c => c.Equals(CapabilityOptions.NoTranslations, StringComparison.OrdinalIgnoreCase)))
            {
                Translations = Container.Get<ITranslations>();
                Translations.Load(new EnvironmentContext()
                {
                    WorkingDirectory = Path.Combine("Plugins", ""),
                    Name = Name + ".Translations"
                }, DefaultTranslations);
            }

            OnLoad();
            IsAlive = true;

            if (eventManager != null)
            {
                PluginLoadedEvent loadedEvent = new PluginLoadedEvent(this);
                eventManager.Emit(runtime, loadedEvent);
            }
        }

        public void Unload()
        {
            IEventManager eventManager = Container.Get<IEventManager>();
            IRuntime runtime = Container.Get<IRuntime>();

            if (eventManager != null)
            {
                PluginUnloadEvent loadedEvent = new PluginUnloadEvent(this);
                eventManager.Emit(runtime, loadedEvent);
            }

            OnUnload();
            IsAlive = false;

            if (eventManager != null)
            {
                PluginUnloadedEvent loadedEvent = new PluginUnloadedEvent(this);
                eventManager.Emit(runtime, loadedEvent);
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