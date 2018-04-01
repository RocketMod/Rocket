using System;

namespace Rocket.API.Eventing
{
    public delegate void EventExecutedCallback(IEvent @event);
    public interface IEventManager
    {
        void Emit(IEventEmitter sender, IEvent @event, EventExecutedCallback cb = null, bool global = true);

        void Emit(IEventEmitter emitter, IEventArgs args, EventExecutedCallback cb = null, bool global = true);

        void Subscribe(ILifecycleObject @object, IEventListener listener, string eventEmitter = null);

        void Subscribe<T>(ILifecycleObject @object, Action<T> callback, string eventEmitter = null) where T: IEvent;

        void Subscribe(ILifecycleObject @object, string eventName, Action<IEvent> callback, string eventEmitter = null);
        
        void Subscribe(ILifecycleObject @object, string eventEmitter = null);

        void Unsubscribe(ILifecycleObject @object, string eventEmitter = null);

        void Unsubscribe<T>(ILifecycleObject @object, string eventEmitter = null) where T : IEvent;

        void Unsubscribe(ILifecycleObject @object, string @event, string eventEmitter = null);

        void Unsubscribe(ILifecycleObject @object, IEventListener listener, string eventEmitter = null);

    }
}