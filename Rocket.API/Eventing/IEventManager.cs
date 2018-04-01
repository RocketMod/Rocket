using System;
using Rocket.API.Plugin;

namespace Rocket.API.Eventing
{
    public interface IEventManager
    {
        void Subscribe(ILifecycleObject @object, IEventListener listener);

        void Unsubscribe(ILifecycleObject @object, IEventListener listener);

        void Subscribe<T>(ILifecycleObject @object, Action<T> callback);

        void Subscribe(ILifecycleObject @object, string eventName, Action<IEvent> callback);

        void Emit(ILifecycleObject sender, IEvent @event);

        void UnsubcribeAllEvents(ILifecycleObject @object);

        void SubscribeAllEvents(ILifecycleObject @object);
    }
}