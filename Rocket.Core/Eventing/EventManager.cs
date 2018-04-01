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
    public class EventManager : IEventManager
    {
        private readonly IDependencyContainer _container;
        private ITaskScheduler Scheduler => _container.Get<ITaskScheduler>();

        public EventManager(IDependencyContainer container)
        {
            _container = container;
        }

        private readonly List<EventAction> _eventListeners = new List<EventAction>();

        public void Subscribe(ILifecycleObject @object, string emitterName = null)
        {
            var listeners = ReflectionHelper.FindTypes<IAutoRegisteredListener>(@object, false);
            foreach (var listener in listeners)
                Subscribe(@object, (IEventListener)Activator.CreateInstance(listener, true), emitterName);
        }

        public void Subscribe(ILifecycleObject @object, IEventListener listener, string emitterName = null)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            RegisterEventsInternal(@object, listener, emitterName);
        }


        public void Subscribe<T>(ILifecycleObject @object, Action<T> callback, string emitterName = null) where T: IEvent
        {
            var handler = (EventHandler)callback.GetType().GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault() ??
                          new EventHandler();

            var eventName = typeof(T).Name.Replace("Event", "");
            EventAction action = new EventAction(@object, (@event) =>
            {
                @callback.Invoke((T)@event);
            }, handler, eventName, emitterName);
            _eventListeners.Add(action);
        }

        public void Subscribe(ILifecycleObject @object, string eventName, Action<IEvent> callback, string emitterName = null)
        {
            var handler = (EventHandler)callback.GetType().GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault() ??
                          new EventHandler();

            EventAction action = new EventAction(@object, callback, handler, eventName, emitterName);

            _eventListeners.Add(action);
        }

        public void Emit(IEventEmitter sender, IEvent @event, EventExecutedCallback cb = null, bool global = true)
        {
            List<EventAction> actions =
                _eventListeners.Where(c => c.TargetEventType.Equals(@event.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            actions.Sort(EventComprarer.Compare);

            var targetActions =
                (from info in actions
                     /* ignore cancelled events */
                 where !(@event is ICancellableEvent)
                       || !((ICancellableEvent)@event).IsCancelled
                       || info.Handler.IgnoreCancelled
                 where CheckEmitter(info, sender.Name)
                 select info)
                .ToList();

            if (targetActions.Count == 0)
            {
                cb?.Invoke(@event);
                return;
            }

            int executionCount = 0;
            foreach (EventAction info in targetActions)
            {
                var pl = info.Owner;
                if (!pl.IsAlive)
                {
                    continue;
                }

                Scheduler.Schedule(pl, () =>
                {
                    executionCount++;
                    info.Invoke(@event);

                    //all actions called; run OnEventExecuted
                    if (executionCount == targetActions.Count)
                        cb?.Invoke(@event);
                }, (ExecutionTargetContext)@event.ExecutionTarget);
            }
        }

        public void Emit(IEventEmitter emitter, IEventArgs args, EventExecutedCallback cb = null, bool global = true)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(ILifecycleObject @object, string emitterName = null)
        {
            _eventListeners.RemoveAll(c => c.Owner == @object && CheckEmitter(c, emitterName));
        }

        public void Unsubscribe<T>(ILifecycleObject @object, string eventEmitter = null) where T : IEvent
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(ILifecycleObject @object, string @event, string eventEmitter = null)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(ILifecycleObject @object, IEventListener listener, string emitterName = null)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            _eventListeners.RemoveAll(c => c.Owner == @object && c.Listener == listener && CheckEmitter(c, emitterName));
        }

        private bool CheckEmitter(EventAction eventAction, string emitterName)
        {
            if (String.IsNullOrEmpty(emitterName))
                return true;

            return eventAction.EmitterName.Equals(emitterName, StringComparison.OrdinalIgnoreCase);
        }

        private void RegisterEventsInternal(ILifecycleObject @object, IEventListener listener, string emitterName = null)
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
                    _eventListeners.Add(new EventAction(@object, listener, method, handler, emitterName));
            }
        }
    }
}