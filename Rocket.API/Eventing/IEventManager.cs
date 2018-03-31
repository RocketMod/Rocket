using System;
using Rocket.API.Plugin;

namespace Rocket.API.Eventing
{
    public interface IEventManager
    {
        void Subscribe(ILifecycleController @object, IEventListener listener);

        void Unsubscribe(ILifecycleController @object, IEventListener listener);

        void Subscribe<T>(ILifecycleController @object, Action<T> callback);

        void Subscribe(ILifecycleController @object, string eventName, Action<Object> callback);

        void Emit(IEventEmitter emitter, IEvent @event);

        void UnsubcribeAllEvents(ILifecycleController @object);

        void SubscribeAllEvents(ILifecycleController @object);
    }
}