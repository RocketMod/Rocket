using System.Collections.Generic;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Plugin;

namespace Rocket.Core.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        protected PluginBase(IDependencyContainer container) : this(null, container)
        {

        }

        protected PluginBase(string name, IDependencyContainer container)
        {
            Name = name ?? GetType().Name;

            Container = container;
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
            OnLoad();
            IsAlive = true;
        }

        public void Unload() {
            OnUnload();
            IsAlive = false;
        }

        public bool IsAlive { get; internal set; }

        protected virtual void OnLoad() { }

        protected virtual void OnUnload() { }

        public void Subscribe<T>(EventCallback<T> callback) where T : IEvent {
            EventManager.Subscribe(this, callback);
        }

        public void Subscribe(string eventName, EventCallback callback) {
            EventManager.Subscribe(this, eventName, callback);
        }

        public void Emit(IEvent @event) {
            EventManager.Emit(this, @event);
        }
    }
}