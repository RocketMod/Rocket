using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Handlers;
using Rocket.API.Scheduler;

namespace Rocket.Core.Eventing
{
    public class EventManager : IEventManager
    {
        private readonly ITaskScheduler scheduler;
        private readonly List<EventAction> eventListeners = new List<EventAction>();

        public EventManager(ITaskScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public void Subscribe(ILifecycleObject @object, string eventName, EventCallback callback)
        {
            EventHandler handler = GetEventHandler(callback.GetType(), null);
            eventListeners.Add(new EventAction(@object, callback.Invoke, handler, eventName));
        }

        public void Subscribe<TEvent>(ILifecycleObject @object, EventCallback<TEvent> callback,
                                      string emitterName = null) where TEvent : IEvent
        {
            EventHandler handler = GetEventHandler(callback.GetType(), emitterName);
            eventListeners.Add(new EventAction(@object,
                (sender, @event) => { callback.Invoke(sender, (TEvent)@event); }, handler, typeof(TEvent)));
        }

        public void Subscribe(ILifecycleObject @object, Type eventType, EventCallback callback,
                              string emitterName = null)
        {
            EventHandler handler = GetEventHandler(callback.GetType(), emitterName);
            eventListeners.Add(new EventAction(@object, callback.Invoke, handler, eventType));
        }

        public void Subscribe(ILifecycleObject @object, string emitterName, string eventName, EventCallback callback)
        {
            EventHandler handler = GetEventHandler(callback.GetType(), emitterName);
            eventListeners.Add(new EventAction(@object, callback, handler, eventName));
        }

        public void Subscribe<TEvent, TEmitter>(ILifecycleObject @object, EventCallback<TEvent> callback)
            where TEvent : IEvent where TEmitter : IEventEmitter
        {
            EventHandler handler = GetEventHandler(callback.GetType(), GetEmitterName(typeof(TEmitter)));
            eventListeners.Add(new EventAction(@object,
                (sender, @event) => { callback.Invoke(sender, (TEvent)@event); }, handler, typeof(TEvent)));
        }

        public void Subscribe(ILifecycleObject @object, EventCallback callback, Type eventType, Type eventEmitterType)
        {
            EventHandler handler = GetEventHandler(callback.GetType(), GetEmitterName(eventEmitterType));
            eventListeners.Add(new EventAction(@object, callback.Invoke, handler, eventType));
        }

        public void Unsubscribe(ILifecycleObject @object)
        {
            eventListeners.RemoveAll(c => c.Owner == @object);
        }

        public void Unsubscribe(ILifecycleObject @object, string eventName)
        {
            eventListeners.RemoveAll(c => c.Owner == @object && CheckEvent(c, eventName));
        }

        public void Unsubscribe<TEvent>(ILifecycleObject @object) where TEvent : IEvent
        {
            eventListeners.RemoveAll(c => c.Owner == @object);
        }

        public void Unsubscribe(ILifecycleObject @object, Type eventType)
        {
            eventListeners.RemoveAll(c => c.Owner == @object && CheckEvent(c, GetEventName(eventType)));
        }

        public void Unsubscribe(ILifecycleObject @object, string emitterName, string eventName)
        {
            eventListeners.RemoveAll(c => c.Owner == @object
                && c.Handler.EmitterName.Equals(emitterName,
                    StringComparison.OrdinalIgnoreCase)
                && CheckEvent(c, eventName));
        }

        public void Unsubscribe<TEvent, TEmitter>(ILifecycleObject @object)
            where TEvent : IEvent where TEmitter : IEventEmitter
        {
            eventListeners.RemoveAll(c => c.Owner == @object
                && CheckEvent(c, GetEventName(typeof(TEvent)))
                && CheckEmitter(c, GetEmitterName(typeof(TEmitter))));
        }

        public void Unsubscribe(ILifecycleObject @object, Type eventType, Type eventEmitterType)
        {
            eventListeners.RemoveAll(c => c.Owner == @object
                && CheckEvent(c, GetEventName(eventType))
                && CheckEmitter(c, GetEmitterName(eventEmitterType)));
        }

        public void AddEventListener(ILifecycleObject @object, IEventListener eventListener)
        {
            // ReSharper disable once UseIsOperator.2
            if (!typeof(IEventListener<>).IsInstanceOfType(eventListener))
                throw new ArgumentException(
                    "The eventListener to register has to implement at least one IEventListener<X>!",
                    nameof(eventListener));

            if (eventListeners.Any(c => c.Listener == eventListener)) return;

            Type type = eventListener.GetType();

            foreach (Type @interface in type.GetInterfaces().Where(c => c == typeof(IEventListener<>)))
                foreach (MethodInfo method in @interface.GetMethods())
                {
                    EventHandler handler = (EventHandler)method.GetCustomAttributes(typeof(EventHandler), false)
                                                                .FirstOrDefault()
                        ?? new EventHandler
                        {
                            Priority = HandlerPriority.Normal
                        };

                    Type eventType = @interface.GetGenericArguments()[0];

                    eventListeners.Add(new EventAction(@object, eventListener, method, handler, eventType));
                }
        }

        public void RemoveEventListener(IEventListener eventListener)
        {
            eventListeners.RemoveAll(c => c.Listener == eventListener);
        }

        public void Emit(IEventEmitter sender, IEvent @event, EventExecutedCallback callback = null)
        {
            List<EventAction> actions =
                eventListeners
                    .Where(c => c.TargetEventType?.IsInstanceOfType(@event)
                        ?? c.TargetEventName.Equals(@event.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            actions.Sort((a, b) => HandlerPriorityComparer.Compare(a.Handler.Priority, b.Handler.Priority));

            List<EventAction> targetActions =
                (from info in actions
                     /* ignore cancelled events */
                 where !(@event is ICancellableEvent)
                     || !((ICancellableEvent)@event).IsCancelled
                     || info.Handler.IgnoreCancelled
                 where CheckEmitter(info, sender.Name)
                 where CheckEvent(info, GetEventName(@event.GetType()))
                 select info)
                .ToList();

            if (targetActions.Count == 0)
            {
                callback?.Invoke(@event);
                return;
            }

            int executionCount = 0;
            foreach (EventAction info in targetActions)
            {
                ILifecycleObject pl = info.Owner;
                if (!pl.IsAlive) continue;

                scheduler.Schedule(pl, () =>
                {
                    executionCount++;
                    info.Action.Invoke(sender, @event);

                    //all actions called; run OnEventExecuted
                    if (executionCount == targetActions.Count) callback?.Invoke(@event);
                }, (ExecutionTargetContext)@event.ExecutionTarget);
            }
        }

        public static string GetEmitterName(Type type) => type.Name;

        public static string GetEventName(Type type) => type.Name.Replace("Event", "");

        private EventHandler GetEventHandler(Type target, string emitterName)
        {
            EventHandler handler =
                (EventHandler)target.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault()
                ?? new EventHandler();
            handler.EmitterName = emitterName ?? handler.EmitterName;
            return handler;
        }

        private bool CheckEmitter(EventAction eventAction, string emitterName)
        {
            if (string.IsNullOrEmpty(emitterName)) return true;

            return eventAction.Handler.EmitterName.Equals(emitterName, StringComparison.OrdinalIgnoreCase);
        }

        private bool CheckEvent(EventAction eventAction, string eventName) => (eventAction.TargetEventType != null
            ? GetEventName(eventAction.TargetEventType)
            : eventAction.TargetEventName)
                .Equals(eventName, StringComparison.OrdinalIgnoreCase);
    }
}