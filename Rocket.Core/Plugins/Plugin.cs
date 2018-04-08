using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using Rocket.Core.I18N;

namespace Rocket.Core.Plugins
{
    public abstract class Plugin : IPlugin
    {
        public class CapabilityOptions
        {
            public const string NoConfig = "NoConfig";
            public const string NoTranslations = "NoTranslations";

            public const string CustomConfig = "CustomConfig";
            public const string CustomTranslations = "CustomTranslations";
        }

        public IConfiguration Configuration { get; protected set; }
        public ITranslations Translations { get; protected set; }


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
            if (!Capabilities.Any(c => c.Equals(CapabilityOptions.CustomConfig, StringComparison.OrdinalIgnoreCase))
             && !Capabilities.Any(c => c.Equals(CapabilityOptions.NoConfig, StringComparison.OrdinalIgnoreCase)))
            {
                Configuration = Container.Get<IConfiguration>();
                //todo: load config
            }

            if (!Capabilities.Any(c => c.Equals(CapabilityOptions.CustomTranslations, StringComparison.OrdinalIgnoreCase))
                && !Capabilities.Any(c => c.Equals(CapabilityOptions.NoTranslations, StringComparison.OrdinalIgnoreCase)))
            {
                Translations = Container.Get<ITranslations>();
                //todo: load config
            }

            OnLoad();
            IsAlive = true;
        }

        public void Unload()
        {
            OnUnload();
            IsAlive = false;
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