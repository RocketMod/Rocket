using Rocket.API.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rocket.API.Eventing
{
    /// <summary>
    ///     The type safe callback for event notifications.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="sender">The event emitter.</param>
    /// <param name="event">The event instance.</param>
    public delegate Task EventCallback<in TEvent>(IEventEmitter sender, TEvent @event) where TEvent : IEvent;

    /// <summary>
    ///     The callback for event notifications.
    /// </summary>
    /// <param name="sender">The event emitter.</param>
    /// <param name="event">The event instance.</param>
    public delegate Task EventCallback(IEventEmitter sender, IEvent @event);

    /// <summary>
    ///     The emit callback for events that have finished and notified all listeners.
    /// </summary>
    /// <param name="event"></param>
    public delegate Task EventExecutedCallback(IEvent @event);

    /// <summary>
    ///     The event manager is responsible for emitting events and for managing their subscriptions.
    /// </summary>
    public interface IEventBus : IService
    {
        /// <summary>
        ///     Subscribe to an event.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="eventName">The event to subscribe to.</param>
        /// <param name="callback">The action to execute. See <see cref="EventCallback" /></param>
        void Subscribe(ILifecycleObject @object, string eventName, EventCallback callback);

        /// <summary>
        ///     <inheritdoc cref="Subscribe(ILifecycleObject,string,EventCallback)" />
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <param name="emitterName">The emitter to subscribe to. Will be a global subscription if the emitterName is null.</param>
        /// <typeparam name="TEvent">The event to subscribe to.</typeparam>
        void Subscribe<TEvent>(ILifecycleObject @object, EventCallback<TEvent> callback, string emitterName = null)
            where TEvent : IEvent;

        /// <summary>
        ///     <inheritdoc cref="Subscribe(ILifecycleObject,string,EventCallback)" />
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <param name="emitterName">The emitter to subscribe to. Will be a global subscription if the emitterName is null.</param>
        /// <param name="eventType">The event to subscribe to.</param>
        void Subscribe(ILifecycleObject @object, Type eventType, EventCallback callback, string emitterName = null);

        /// <summary>
        ///     <inheritdoc cref="Subscribe(ILifecycleObject,string,EventCallback)" />
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <param name="emitterName">The emitter to subscribe to. Will be a global subscription if the emitterName is null.</param>
        /// <param name="eventName">The event to subscribe to.</param>
        void Subscribe(ILifecycleObject @object, string emitterName, string eventName, EventCallback callback);

        /// <summary>
        ///     <inheritdoc cref="Subscribe(ILifecycleObject,string,EventCallback)" />
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <typeparam name="TEvent">The event to subscribe to.</typeparam>
        /// <typeparam name="TEmitter">The emitter to subscribe to.</typeparam>
        void Subscribe<TEvent, TEmitter>(ILifecycleObject @object, EventCallback<TEvent> callback)
            where TEvent : IEvent where TEmitter : IEventEmitter;

        /// <summary>
        ///     <inheritdoc cref="Subscribe(ILifecycleObject,string,EventCallback)" />
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <param name="eventType">The event to subscribe to.</param>
        /// <param name="emitterType">The emitter to subscribe to. Will be a global subscription if the emitterType is null.</param>
        void Subscribe(ILifecycleObject @object, EventCallback callback, Type eventType, Type emitterType);

        /// <summary>
        ///     Unsubscribe all listener subscriptions of the given lifecycle object.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        void Unsubscribe(ILifecycleObject @object);

        /// <summary>
        ///     Unsubscribe all subscriptions for the given event type of the given lifecycle object.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="eventName">The event to unsubscribe from. Will unsubscribe globally if emitterName is null.</param>
        void Unsubscribe(ILifecycleObject @object, string eventName);

        /// <summary>
        ///     Unsubscribe the event from this lifecycle object type-safe
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        void Unsubscribe<TEvent>(ILifecycleObject @object) where TEvent : IEvent;

        /// <summary>
        ///     Unsubscribe all subscriptions for the given event type of the given lifecycle object.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="eventType">The event to unsubscribe from.</param>
        void Unsubscribe(ILifecycleObject @object, Type eventType);

        /// <summary>
        ///     Unsubscribe an event subscription of the given lifecycle object.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="emitterName">The emitter to unsubscribe from. Will unsubscribe globally if emitterName is null.</param>
        /// <param name="eventName">The event to unsubscribe from. Will unsubscribe from all event types if eventName is null.</param>
        void Unsubscribe(ILifecycleObject @object, string emitterName, string eventName);

        /// <summary>
        ///     Unsubscribe an event subscription of the given lifecycle object from a specific emitter.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        void Unsubscribe<TEvent, TEmitter>(ILifecycleObject @object)
            where TEvent : IEvent where TEmitter : IEventEmitter;

        /// <summary>
        ///     Unsubscribe an event subscription of the given lifecycle object from a specific emitter.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="eventType">The event to unsubscribe from. . Will unsubscribe from all event types if eventType is null.</param>
        /// <param name="eventEmitterType">The emitter to unsubscribe from. Will unsubscribe globally if eventEmitterType is null.</param>
        void Unsubscribe(ILifecycleObject @object, Type eventType, Type eventEmitterType);

        /// <summary>
        ///     Register an event listener instance.
        /// </summary>
        /// <param name="object">The associated lifecycle object.</param>
        /// <param name="eventListener">The event listener to register.</param>
        void AddEventListener(ILifecycleObject @object, IEventListener eventListener);

        /// <summary>
        ///     Remove an event listeners subscription.
        /// </summary>
        /// <param name="eventListener">The event listener to remove.</param>
        void RemoveEventListener(IEventListener eventListener);

        /// <summary>
        ///     Emits an event and optionally handles the result
        /// </summary>
        /// <param name="sender">The event emitter.</param>
        /// <param name="event">The event instance.</param>
        /// <param name="callback">The event finish callback. See <see cref="EventExecutedCallback" />.</param>
        void Emit(IEventEmitter sender, IEvent @event, EventExecutedCallback callback = null);
    }
}