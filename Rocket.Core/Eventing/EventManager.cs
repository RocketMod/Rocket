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
        public void Subscribe(ILifecycleController @object, IEventListener listener)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(ILifecycleController @object, IEventListener listener)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(ILifecycleController @object, Action<T> callback)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(ILifecycleController @object, string eventName, Action<object> callback)
        {
            throw new NotImplementedException();
        }

        public void Emit(IEventEmitter emitter, IEvent @event)
        {
            throw new NotImplementedException();
        }

        public void UnsubcribeAllEvents(ILifecycleController @object)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAllEvents(ILifecycleController @object)
        {
            throw new NotImplementedException();
        }
    }
}
