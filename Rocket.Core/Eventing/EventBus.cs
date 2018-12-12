using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Scheduling;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Eventing
{
    public class EventBus : IEventBus
    {
        private readonly IDependencyContainer container;
        private readonly List<EventAction> eventListeners = new List<EventAction>();

        private readonly List<IEvent> inProgress = new List<IEvent>();
        private readonly ILogger logger;

        public EventBus(IDependencyContainer container)
        {
            this.container = container;
            container.TryResolve(null, out logger);
        }

        public void Subscribe(ILifecycleObject @object, string eventName, EventCallback callback)
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, null);
            eventListeners.Add(new EventAction(@object, callback.Invoke, handler, new List<string> { eventName }));
        }

        public void Subscribe<TEvent>(ILifecycleObject @object, EventCallback<TEvent> callback,
                                      string emitterName = null) where TEvent : IEvent
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, emitterName);
            eventListeners.Add(new EventAction(@object,
                (sender, @event) => callback.Invoke(sender, (TEvent)@event), handler, typeof(TEvent)));
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
            eventListeners.Add(new EventAction(@object, callback, handler,  new List<string> { eventName }));
        }

        public void Subscribe<TEvent, TEmitter>(ILifecycleObject @object, EventCallback<TEvent> callback)
            where TEvent : IEvent where TEmitter : IEventEmitter
        {
            if (!@object.IsAlive)
                return;

            EventHandler handler = GetEventHandler(callback.Method, GetEmitterName(typeof(TEmitter)));
            eventListeners.Add(new EventAction(@object,
                (sender, @event) => callback.Invoke(sender, (TEvent)@event), handler, typeof(TEvent)));
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

            eventListeners.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == @object);
        }

        public void Unsubscribe(ILifecycleObject @object, string eventName)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == @object && CheckEvent(c, new List<string> { eventName }));
        }

        public void Unsubscribe<TEvent>(ILifecycleObject @object) where TEvent : IEvent
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == @object);
        }

        public void Unsubscribe(ILifecycleObject @object, Type eventType)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == @object && CheckEvent(c, GetEventNames(eventType)));
        }

        public void Unsubscribe(ILifecycleObject @object, string emitterName, string eventName)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == @object
                && c.Handler.EmitterName.Equals(emitterName,
                    StringComparison.OrdinalIgnoreCase)
                && CheckEvent(c, new List<string> { eventName }));
        }

        public void Unsubscribe<TEvent, TEmitter>(ILifecycleObject @object)
            where TEvent : IEvent where TEmitter : IEventEmitter
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == @object
                && CheckEvent(c, GetEventNames(typeof(TEvent)))
                && CheckEmitter(c, GetEmitterName(typeof(TEmitter)), false));
        }

        public void Unsubscribe(ILifecycleObject @object, Type eventType, Type eventEmitterType)
        {
            if (!@object.IsAlive)
                return;

            eventListeners.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == @object
                && CheckEvent(c, GetEventNames(eventType))
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

            if (eventListeners.Any(c => c.Listener?.GetType() == eventListener.GetType())) return;

            Type type = eventListener.GetType();

            foreach (Type @interface in type.GetInterfaces()
                                            .Where(c => typeof(IEventListener).IsAssignableFrom(c)
                                                && c.GetGenericArguments().Length > 0))
                foreach (MethodInfo method in @interface.GetMethods())
                {
                    EventHandler handler = (EventHandler)method.GetCustomAttributes(typeof(EventHandler), false)
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

            string eventNameString = "[" + string.Join(", ", @event.Names.ToArray()) + "]" + " by \"" + sender.Name + "\"";
            string primaryName = @event.Names.First();

            logger?.LogTrace(eventNameString + ": Emitting.");

            inProgress.Add(@event);

            List<EventAction> actions =
                eventListeners
                    .Where(c => c.TargetEventType?.IsInstanceOfType(@event)
                        ?? @event.Names.Any(d => c.TargetEventNames.Any(e => d.Equals(e, StringComparison.OrdinalIgnoreCase)))
                        && c.Owner.IsAlive)
                    .ToList();

            actions.Sort((a, b) => ServicePriorityComparer.Compare(a.Handler.Priority, b.Handler.Priority));

            List<EventAction> targetActions =
                (from info in actions
                     /* ignore cancelled events */
                 where !(@event is ICancellableEvent)
                     || !((ICancellableEvent)@event).IsCancelled
                     || info.Handler.IgnoreCancelled
                 where CheckEmitter(info, sender.Name, @event.IsGlobal)
                 where CheckEvent(info, @event.Names)
                 select info)
                .ToList();

            void FinishEvent()
            {
                logger?.LogTrace(eventNameString + ": Finished.");
                inProgress.Remove(@event);
                callback?.Invoke(@event);
            }

            if (targetActions.Count == 0)
            {
                logger?.LogTrace(eventNameString + ": No listeners found.");
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
                Util.WeakReference<ILifecycleObject> wk = info.Owner;
                if (!wk.IsAlive)
                {
                    actions.Remove(info);
                    continue;
                }

                var pl = wk.Target;

                if (scheduler == null)
                {
                    info.Action.Invoke(sender, @event);
                    continue;
                }

                scheduler.ScheduleUpdate(pl, async () =>
                {
                    executionCount++;
                    await info.Action.Invoke(sender, @event);

                    //all actions called; run OnEventExecuted
                    if (executionCount == targetActions.Count) FinishEvent();
                }, primaryName + "EmitTask", (ExecutionTargetContext)@event.ExecutionTarget);
            }

            if (scheduler == null) FinishEvent();
        }

        public bool HasFinished(IEvent @event) => !inProgress.Contains(@event);

        public static string GetEmitterName(Type type) => type.Name;

        private EventHandler GetEventHandler(Type target, string emitterName)
        {
            EventHandler handler =
                (EventHandler)target?.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault()
                ?? new EventHandler();
            handler.EmitterName = emitterName ?? handler.EmitterName;
            return handler;
        }

        private EventHandler GetEventHandler(MethodInfo target, string emitterName)
        {
            EventHandler handler =
                (EventHandler)target.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault()
                ?? GetEventHandler(target.DeclaringType, emitterName);

            handler.EmitterName = emitterName ?? handler.EmitterName;
            return handler;
        }

        private bool CheckEmitter(EventAction eventAction, string emitterName, bool isGlobal)
        {
            if (string.IsNullOrEmpty(emitterName))
            {
                logger.LogTrace("CheckEmitter: requested emitterName is null or empty; returning true");
                return true;
            }

            if (isGlobal && string.IsNullOrEmpty(eventAction.Handler.EmitterName))
            {
                logger.LogTrace("CheckEmitter: isGlobal and handler emitter name is null; returning true");
                return true;
            }

            if (eventAction.Handler.EmitterName?.Equals(emitterName, StringComparison.OrdinalIgnoreCase) ?? true)
            {
                logger.LogTrace($"CheckEmitter: {eventAction.Handler.EmitterName ?? "null"} == {emitterName}; returning true");
                return true;
            }

            logger.LogTrace($"CheckEmitter: {eventAction.Handler.EmitterName ?? "null"} != {emitterName}; returning false");
            return false;
        }

        private bool CheckEvent(EventAction eventAction, IEnumerable<string> eventNames)
        {
            if (eventAction.TargetEventType == null)
            {
                return eventNames.Any(c => eventAction.TargetEventNames.Any(e => c.Equals(e, StringComparison.OrdinalIgnoreCase)));
            }

            return eventNames.Any(c => GetEventNames(eventAction.TargetEventType).Any(d => d.Equals(c, StringComparison.OrdinalIgnoreCase)));
        }

        public static List<string> GetEventNames(Type t)
        {
            List<string> names = new List<string>();
            foreach (var type in t.GetTypeHierarchy())
            {
                if (!typeof(IEvent).IsAssignableFrom(type))
                    break;

                if (type == typeof(Event))
                    break;

                var attr = type.GetCustomAttributes(typeof(EventNameAttribute), false)
                               .Cast<EventNameAttribute>()
                               .ToList();
                if (attr.Count == 0)
                {
                    names.Add(type.Name.Replace("Event", ""));
                    continue;
                }

                names.AddRange(attr.Select(c => c.EventName));
            }

            return names;
        }
    }
}