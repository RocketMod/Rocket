using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Scheduler;
using Rocket.Core.Logging;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Eventing
{
    public class EventManager : IEventManager
    {
        private readonly IDependencyContainer container;
        private readonly List<EventAction> eventListeners = new List<EventAction>();

        private readonly List<IEvent> inProgress = new List<IEvent>();

        public EventManager(IDependencyContainer container)
        {
            this.container = container;
        }

        public void Subscribe(ILifecycleObject @object, string eventName, EventCallback callback)
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, null);
            eventListeners.Add(new EventAction(@object, callback.Invoke, handler, eventName));
        }

        public void Subscribe<TEvent>(ILifecycleObject @object, EventCallback<TEvent> callback,
                                      string emitterName = null) where TEvent : IEvent
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, emitterName);
            eventListeners.Add(new EventAction(@object,
                (sender, @event) => callback.Invoke(sender, (TEvent) @event), handler, typeof(TEvent)));
        }

        public void Subscribe(ILifecycleObject @object, Type eventType, EventCallback callback,
                              string emitterName = null)
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, emitterName);
            eventListeners.Add(new EventAction(@object, callback.Invoke, handler, eventType));
        }

        public void Subscribe(ILifecycleObject @object, string emitterName, string eventName, EventCallback callback)
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, emitterName);
            eventListeners.Add(new EventAction(@object, callback, handler, eventName));
        }

        public void Subscribe<TEvent, TEmitter>(ILifecycleObject @object, EventCallback<TEvent> callback)
            where TEvent : IEvent where TEmitter : IEventEmitter
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, GetEmitterName(typeof(TEmitter)));
            eventListeners.Add(new EventAction(@object,
                (sender, @event) => callback.Invoke(sender, (TEvent) @event), handler, typeof(TEvent)));
        }

        public void Subscribe(ILifecycleObject @object, EventCallback callback, Type eventType, Type eventEmitterType)
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, GetEmitterName(eventEmitterType));
            eventListeners.Add(new EventAction(@object, callback.Invoke, handler, eventType));
        }

        public void Unsubscribe(ILifecycleObject @object)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => c.Owner == @object);
        }

        public void Unsubscribe(ILifecycleObject @object, string eventName)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => c.Owner == @object && CheckEvent(c, eventName));
        }

        public void Unsubscribe<TEvent>(ILifecycleObject @object) where TEvent : IEvent
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => c.Owner == @object);
        }

        public void Unsubscribe(ILifecycleObject @object, Type eventType)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => c.Owner == @object && CheckEvent(c, GetEventName(eventType)));
        }

        public void Unsubscribe(ILifecycleObject @object, string emitterName, string eventName)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => c.Owner == @object
                && c.Handler.EmitterName.Equals(emitterName,
                    StringComparison.OrdinalIgnoreCase)
                && CheckEvent(c, eventName));
        }

        public void Unsubscribe<TEvent, TEmitter>(ILifecycleObject @object)
            where TEvent : IEvent where TEmitter : IEventEmitter
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => c.Owner == @object
                && CheckEvent(c, GetEventName(typeof(TEvent)))
                && CheckEmitter(c, GetEmitterName(typeof(TEmitter)), false));
        }

        public void Unsubscribe(ILifecycleObject @object, Type eventType, Type eventEmitterType)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => c.Owner == @object
                && CheckEvent(c, GetEventName(eventType))
                && CheckEmitter(c, GetEmitterName(eventEmitterType), false));
        }

        public void AddEventListener(ILifecycleObject @object, IEventListener eventListener)
        {
            if (!@object.IsAlive)
                return;

            // ReSharper disable once UseIsOperator.2
            if (!typeof(IEventListener).IsInstanceOfType(eventListener))
                throw new ArgumentException(
                    "The eventListener to register has to implement IEventListener!",
                    nameof(eventListener));

            if (eventListeners.Any(c => c.Listener.GetType() == eventListener.GetType())) return;

            Type type = eventListener.GetType();

            foreach (Type @interface in type.GetInterfaces()
                                            .Where(c => typeof(IEventListener).IsAssignableFrom(c)
                                                && c.GetGenericArguments().Length > 0))
                foreach (MethodInfo method in @interface.GetMethods())
                {
                    EventHandler handler = (EventHandler) method.GetCustomAttributes(typeof(EventHandler), false)
                                                                .FirstOrDefault()
                        ?? new EventHandler
                        {
                            Priority = ServicePriority.Normal
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
            if (!sender.IsAlive)
                return;

            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            container.TryResolve(null, out ILogger logger);
            logger?.LogDebug("Emitting event: \"" + @event.Name + "\" by \"" + sender.Name + "\"");

            inProgress.Add(@event);

            List<EventAction> actions =
                eventListeners
                    .Where(c => c.TargetEventType?.IsInstanceOfType(@event)
                        ?? c.TargetEventName.Equals(@event.Name, StringComparison.OrdinalIgnoreCase)
                        && c.Owner.IsAlive)
                    .ToList();

            actions.Sort((a, b) => ServicePriorityComparer.Compare(a.Handler.Priority, b.Handler.Priority));

            List<EventAction> targetActions =
                (from info in actions
                 /* ignore cancelled events */
                 where !(@event is ICancellableEvent)
                     || !((ICancellableEvent) @event).IsCancelled
                     || info.Handler.IgnoreCancelled
                 where CheckEmitter(info, sender.Name, @event.IsGlobal)
                 where CheckEvent(info, GetEventName(@event.GetType()))
                 select info)
                .ToList();

            void FinishEvent()
            {
                logger?.LogDebug("Event finished: \"" + @event.Name + "\" by \"" + sender.Name + "\"");
                inProgress.Remove(@event);
                callback?.Invoke(@event);
            }

            if (targetActions.Count == 0)
            {
                logger?.LogDebug("Omitting event \""
                    + @event.Name
                    + "\" by \""
                    + sender.Name
                    + "\": No target subscriptions found.");
                FinishEvent();
                return;
            }

            container.TryResolve(null, out ITaskScheduler scheduler);
            if (scheduler == null && @event.ExecutionTarget != EventExecutionTargetContext.Sync)
            {
                FinishEvent();
                return;
            }

            int executionCount = 0;
            foreach (EventAction info in targetActions)
            {
                ILifecycleObject pl = info.Owner;

                if (scheduler == null)
                {
                    info.Action.Invoke(sender, @event);
                    continue;
                }

                scheduler.Schedule(pl, () =>
                {
                    executionCount++;
                    info.Action.Invoke(sender, @event);

                    //all actions called; run OnEventExecuted
                    if (executionCount == targetActions.Count) FinishEvent();
                }, (ExecutionTargetContext) @event.ExecutionTarget);
            }

            if (scheduler == null) FinishEvent();
        }

        public bool HasFinished(IEvent @event) => !inProgress.Contains(@event);

        public static string GetEmitterName(Type type) => type.Name;

        public static string GetEventName(Type type) => type.Name.Replace("Event", "");

        private EventHandler GetEventHandler(Type target, string emitterName)
        {
            EventHandler handler =
                (EventHandler) target.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault()
                ?? new EventHandler();
            handler.EmitterName = emitterName ?? handler.EmitterName;
            return handler;
        }

        private EventHandler GetEventHandler(MethodInfo target, string emitterName)
        {
            EventHandler handler =
                (EventHandler) target.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault()
                ?? GetEventHandler(target.DeclaringType, emitterName);

            handler.EmitterName = emitterName ?? handler.EmitterName;
            return handler;
        }

        private bool CheckEmitter(EventAction eventAction, string emitterName, bool isGlobal)
        {
            if (string.IsNullOrEmpty(emitterName))
                return true;

            if (isGlobal && string.IsNullOrEmpty(eventAction.Handler.EmitterName))
                return true;

            return eventAction.Handler.EmitterName?.Equals(emitterName, StringComparison.OrdinalIgnoreCase) ?? true;
        }

        private bool CheckEvent(EventAction eventAction, string eventName) => (eventAction.TargetEventType != null
                ? GetEventName(eventAction.TargetEventType)
                : eventAction.TargetEventName)
            .Equals(eventName, StringComparison.OrdinalIgnoreCase);
    }
}