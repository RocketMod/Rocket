using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Scheduler;
using EventHandler = Rocket.API.Eventing.EventHandler;
using ReflectionHelper = Rocket.API.Reflection.ReflectionHelper;

namespace Rocket.Core.Eventing
{
    public class EventAction
    {
        public EventAction(ILifecycleObject owner, Action<IEvent> action, EventHandler handler, string eventName)
        {
            Owner = owner;
            Action = action;
            Handler = handler;
            TargetEventType = eventName;
        }

        public EventAction(
            ILifecycleObject owner,
            IEventListener listener,
            MethodInfo method,
            EventHandler handler)
        {
            Owner = owner;
            Listener = listener;
            Method = method;
            Handler = handler;

            if (method.GetParameters().Length != 1 || !typeof(IEvent).IsAssignableFrom(method.GetParameters()[0].ParameterType))
                throw new Exception("Method: " + method.Name + " in type " + method.DeclaringType.FullName + " does not have correct signature for events!");
            Type targetType = method.GetParameters()[0].ParameterType;
            TargetEventType = targetType.Name.Replace("Event", "");
        }

        public Action<IEvent> Action { get; }

        public MethodInfo Method { get; }

        public EventHandler Handler { get; }

        public IEventListener Listener { get; }

        public ILifecycleObject Owner { get; }

        public string TargetEventType { get; }

        public void Invoke(IEvent @event)
        {
            Action?.Invoke(@event);
            Method?.Invoke(Owner, new object[] { @event });
        }
    }

    public class EventManager : IEventManager
    {
        private readonly IDependencyContainer _container;
        private ITaskScheduler Scheduler => _container.Get<ITaskScheduler>();

        public EventManager(IDependencyContainer container)
        {
            _container = container;
        }

        private readonly List<EventAction> _eventListeners = new List<EventAction>();

        public void Subscribe(ILifecycleObject @object, IEventListener listener)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            RegisterEventsInternal(@object, listener);
        }

        internal void RegisterEventsInternal(ILifecycleObject @object, IEventListener listener)
        {

            Type type = listener.GetType();
            foreach (MethodInfo method in type.GetMethods())
            {
                var handler = (EventHandler)method.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault();

                if (handler == null)
                {
                    continue;
                }

                ParameterInfo[] methodArgs = method.GetParameters();
                if (methodArgs.Length != 1)
                {
                    //Listener methods should have only one argument
                    continue;
                }

                Type t = methodArgs[0].ParameterType;
                if (!t.IsSubclassOf(typeof(Event)))
                {
                    //The arg type should be instanceof Event
                    continue;
                }

                if (_eventListeners.All(c => c.Method != method))
                    _eventListeners.Add(new EventAction(@object, listener, method, handler));
            }
        }

        public void Unsubscribe(ILifecycleObject @object, IEventListener listener)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            _eventListeners.RemoveAll(c => c.Owner == @object && c.Listener == listener);
        }

        public void Subscribe<T>(ILifecycleObject @object, Action<T> callback)
        {
            var handler = (EventHandler)callback.GetType().GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault() ??
                          new EventHandler();

            var eventName = typeof(T).Name.Replace("Event", "");
            EventAction action = new EventAction(@object, (@event) =>
            {
                @callback.Invoke((T)@event);
            }, handler, eventName);
            _eventListeners.Add(action);
        }

        public void Subscribe(ILifecycleObject @object, string eventName, Action<IEvent> callback)
        {
            var handler = (EventHandler)callback.GetType().GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault() ??
                          new EventHandler();

            EventAction action = new EventAction(@object, callback, handler, eventName);

            _eventListeners.Add(action);
        }

        public void Emit(ILifecycleObject sender, IEvent @event)
        {
            List<EventAction> actions =
                _eventListeners.Where(c => c.TargetEventType.Equals(@event.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            actions.Sort(EventComprarer.Compare);

            foreach (EventAction info in from info in actions
                                             /* ignore cancelled events */
                                         where !(@event is ICancellableEvent)
                                               || !((ICancellableEvent)@event).IsCancelled
                                               || info.Handler.IgnoreCancelled
                                         select info)
            {
                var pl = info.Owner;
                if (!pl.IsAlive)
                {
                    continue;
                }

                Scheduler.Schedule(pl, () => { info.Invoke(@event); }, (ExecutionTargetContext) @event.ExecutionTarget);
            }
        }

        public void UnsubcribeAllEvents(ILifecycleObject @object)
        {
            _eventListeners.RemoveAll(c => c.Owner == @object);
        }

        public void SubscribeAllEvents(ILifecycleObject @object)
        {
            var listeners = ReflectionHelper.FindTypes<IAutoRegisteredListener>(@object, false);
            foreach (var listener in listeners)
                Subscribe(@object, (IEventListener)Activator.CreateInstance(listener, true));
        }
    }
}