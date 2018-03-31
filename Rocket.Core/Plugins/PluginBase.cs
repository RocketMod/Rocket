using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        public IDependencyContainer Container { get; private set; }
        public IEventManager EventManager { get; private set; }
        public ILogger Logger { get; private set; }

        public abstract IEnumerable<string> Capabilities { get; }

        public abstract string Name { get; }

        public PluginBase(IDependencyContainer container)
        {
            Container = container;
            EventManager = Container.Get<IEventManager>();
            Logger = Container.Get<ILogger>();
        }

        public abstract void Load();

        public abstract void Unload();

        public void Subscribe<T>(Type @event, Action<T> callback) where T : IEventArguments => EventManager.Subscribe<T>(this, @event, callback);

        public void Subscribe(string eventName, Action<IEventArguments> callback) => EventManager.Subscribe(this, eventName, callback);
      
        public void Emit(IEvent @event) => EventManager.Emit(this, @event);

        public void Emit(string eventName, IEventArguments arguments) => EventManager.Emit(this, eventName, arguments);
    }
}
