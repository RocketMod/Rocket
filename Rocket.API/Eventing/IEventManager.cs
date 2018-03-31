using System;
using Rocket.API.Plugin;

namespace Rocket.API.Eventing
{
    public interface IEventManager
    {
        /// <summary>
        /// Global subscription type-safe
        /// </summary>
        /// <typeparam name="T">IEventArguments that are coming back from this event</typeparam>
        /// <param name="listener">Calling object (subscription lifetime is bound to this object)</param>
        /// <param name="event">Type deriving from IEvent to listen for</param>
        /// <param name="callback">Action to call when the event is triggered</param>
        void Subscribe<T>(ILifecycleObject listener, Type @event, Action<T> callback) where T : IEventArguments;

        /// <summary>
        /// Global subscription by name
        /// </summary>
        /// <param name="listener">Calling object (subscription lifetime is bound to this object)</param>
        /// <param name="eventName">Name of the event to listen for</param>
        /// <param name="callback">Action to call when the event is triggered</param>
        void Subscribe(ILifecycleObject listener, string eventName, Action<IEventArguments> callback);

        /// <summary>
        /// IEventEmitter subscription type-safe
        /// </summary>
        /// <typeparam name="T">IEventArguments that are coming back from this event</typeparam>
        /// <param name="listener">Calling object (subscription lifetime is bound to this object)</param>
        /// <param name="emitter">Sender of this event</param>
        /// <param name="event">Type deriving from IEvent to listen for</param>
        /// <param name="callback">Action to call when the event is triggered</param>
        void Subscribe<T>(ILifecycleObject listener, IEventEmitter emitter, Type @event, Action<T> callback) where T : IEventArguments;

        /// <summary>
        /// IEventEmitter subscription by name
        /// </summary>
        /// <param name="listener">Calling object (subscription lifetime is bound to this object)</param>
        /// <param name="emitterName">Name of the sender of this event</param>
        /// <param name="eventName">Name of the event to listen for</param>
        /// <param name="callback">Action to call when the event is triggered</param>
        void Subscribe(ILifecycleObject listener, string emitterName, string eventName, Action<IEventArguments> callback);
        
        /// <summary>
        /// Emit a new event from an event emitter
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="event">Event including arguments</param>
        void Emit(IEventEmitter sender, IEvent @event);

        /// <summary>
        /// Emit a new event without explicit specifing event emitter
        /// </summary>
        /// <param name="emitterName">Sender of this event</param>
        /// <param name="event">Event including arguments</param>
        void Emit(string emitterName, IEvent @event);

        /// <summary>
        /// Emit a new event without specifing explicit event
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="eventName">Event name</param>
        /// <param name="arguments">Arguments</param>
        void Emit(IEventEmitter sender, string eventName, IEventArguments arguments);

        /// <summary>
        /// Emit a new event without specifing explicit event emitter or event
        /// </summary>
        /// <param name="emitterName">Sender of this event</param>
        /// <param name="eventName">Event name</param>
        /// <param name="arguments">Arguments</param>
        void Emit(string emitterName, string eventName, IEventArguments arguments);
    }
}