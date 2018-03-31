using Rocket.API.Eventing;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace Rocket.Core.Eventing
{
    public class EventManager : IEventManager
    {
        public void Emit(IEventEmitter sender, IEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Emit(string emitterName, IEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Emit(IEventEmitter sender, string eventName, IEventArguments arguments)
        {
            throw new NotImplementedException();
        }

        public void Emit(string emitterName, string eventName, IEventArguments arguments)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(ILifecycleObject listener, Type @event, Action<T> callback) where T : IEventArguments
        {
            throw new NotImplementedException();
        }

        public void Subscribe(ILifecycleObject listener, string eventName, Action<IEventArguments> callback)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(ILifecycleObject listener, IEventEmitter emitter, Type @event, Action<T> callback) where T : IEventArguments
        {
            throw new NotImplementedException();
        }

        public void Subscribe(ILifecycleObject listener, string emitterName, string eventName, Action<IEventArguments> callback)
        {
            throw new NotImplementedException();
        }
    }
}
