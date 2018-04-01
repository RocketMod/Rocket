using System;

namespace Rocket.API.Eventing
{
    public delegate void EventExecutedCallback(IEvent @event);
    public interface IEventManager
    {
        void Subscribe(ILifecycleObject @object, IEventListener listener);

        void Unsubscribe(ILifecycleObject @object, IEventListener listener);

        void Subscribe<T>(ILifecycleObject @object, Action<T> callback);

        void Subscribe(ILifecycleObject @object, string eventName, Action<IEvent> callback);

        void Emit(ILifecycleObject sender, IEvent @event, EventExecutedCallback cb = null);

        void UnsubcribeAllEvents(ILifecycleObject @object);

        void SubscribeAllEvents(ILifecycleObject @object);
    }
}