using System;

namespace Rocket.API.Eventing {
    public delegate void EventCallback<TEvent>(IEventEmitter sender, TEvent arguments) where TEvent : IEvent;

    public delegate void EventCallback(IEventEmitter sender, IEvent @event);

    public delegate void EventExecutedCallback(IEvent @event);

    public interface IEventManager {
        /// <summary>
        ///     Global subscription by name
        /// </summary>
        void Subscribe(ILifecycleObject @object, string eventName, EventCallback callback);

        /// <summary>
        ///     Global subscription type-safe
        /// </summary>
        void Subscribe<TEvent>(ILifecycleObject listener, EventCallback<TEvent> callback, string emitterName = null)
            where TEvent : IEvent;

        void Subscribe(ILifecycleObject @object, Type eventType, EventCallback callback, string emitterName = null);

        /// <summary>
        ///     Subscribe to emitter by name
        /// </summary>
        void Subscribe(ILifecycleObject @object, string emitterName, string eventName, EventCallback callback);

        /// <summary>
        ///     Subscribe to emitter type-safe
        /// </summary>
        void Subscribe<TEvent, TEmitter>(ILifecycleObject listener, EventCallback<TEvent> callback)
            where TEvent : IEvent where TEmitter : IEventEmitter;

        void Subscribe(ILifecycleObject @object, EventCallback callback, Type eventType, Type eventEmitterType);

        /// <summary>
        ///     Unsubscribe all events from this lifecycle object
        /// </summary>
        void Unsubscribe(ILifecycleObject @object);

        /// <summary>
        ///     Unsubscribe the event from this lifecycle object by name
        /// </summary>
        void Unsubscribe(ILifecycleObject @object, string eventName);

        /// <summary>
        ///     Unsubscribe the event from this lifecycle object type-safe
        /// </summary>
        void Unsubscribe<TEvent>(ILifecycleObject listener) where TEvent : IEvent;

        void Unsubscribe(ILifecycleObject @object, Type eventType);

        /// <summary>
        ///     Unsubscribe the event from an emitter from this lifecycle object by name
        /// </summary>
        void Unsubscribe(ILifecycleObject @object, string emitterName, string eventName);

        /// <summary>
        ///     Unsubscribe the event from an emitter from this lifecycle object type-safe
        /// </summary>
        void Unsubscribe<TEvent, TEmitter>(ILifecycleObject listener)
            where TEvent : IEvent where TEmitter : IEventEmitter;

        void Unsubscribe(ILifecycleObject @object, Type eventType, Type eventEmitterType);

        /// <summary>
        ///     Add event listener class (depending on the IEventManager implementation it will automatically register methods
        ///     with the EventHandlerAttribute matching the EventCallback signature in that class)
        /// </summary>
        void AddEventListener(ILifecycleObject @object, IEventListener eventListener);

        /// <summary>
        ///     Remove event listener class
        /// </summary>
        void RemoveEventListener(IEventListener eventListener);

        /// <summary>
        ///     Emits an event and optionally handles the result
        /// </summary>
        void Emit(IEventEmitter sender, IEvent @event, EventExecutedCallback callback = null);
    }
}