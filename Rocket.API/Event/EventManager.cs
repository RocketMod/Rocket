using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Rocket.API.Plugins;

namespace Rocket.API.Event
{
    public class EventManager
    {
        private static EventManager _instance;
        private readonly Dictionary<Type, List<MethodInfo>> _eventListeners = new Dictionary<Type, List<MethodInfo>>();
        private readonly Dictionary<IListener, List<MethodInfo>> _listenerMethods = new Dictionary<IListener, List<MethodInfo>>();
        private readonly Dictionary<IRocketPlugin, List<IListener>> _listeners = new Dictionary<IRocketPlugin, List<IListener>>();
        public static EventManager Instance => _instance ?? (_instance = new EventManager());

        /// <summary>
        /// Register a listener for events
        /// </summary>
        /// <param name="listener">The listener class which implements the <see cref="EventHandler"/> listener methods</param>
        /// <param name="plugin">The plugin which wants to register a new listener</param>
        public void RegisterEvents(IListener listener, IRocketPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            RegisterEventsInternal(listener, plugin);
        }

        internal void RegisterEventsInternal(IListener listener, IRocketPlugin plugin)
        {
            if (!_listeners.ContainsKey(plugin))
            {
                _listeners.Add(plugin, new List<IListener>());
            }
            if (!_listeners[plugin].Contains(listener))
            {
                _listeners[plugin].Add(listener);
            }

            Type type = listener.GetType();
            foreach (MethodInfo method in type.GetMethods())
            {
                bool isEventMethod = method.GetCustomAttributes(false).OfType<System.EventHandler>().Any();

                if (!isEventMethod)
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

                List<MethodInfo> methods;
                try
                {
                    methods = _eventListeners[t];
                }
                catch (KeyNotFoundException)
                {

                    methods = new List<MethodInfo>();
                }
                if (!methods.Contains(method)) methods.Add(method);

                if (_eventListeners.ContainsKey(t))
                {
                    _eventListeners[t] = methods;
                }
                else
                {
                    _eventListeners.Add(t, methods);
                }

                if (!_listenerMethods.ContainsKey(listener))
                {
                    _listenerMethods.Add(listener, new List<MethodInfo>());
                }

                if (!_listenerMethods[listener].Contains(method)) _listenerMethods[listener].Add(method);
            }
        }

        /// <summary>
        /// Call an event which will be send to all <see cref="IListener"/>s which are listening for it
        /// </summary>
        /// <param name="event">The event to fire</param>
        public void CallEvent(Event @event)
        {
            Type t = @event.GetType();
            List<MethodInfo> methods;

            try
            {
                methods = _eventListeners[t];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            methods.Sort(EventComprarer.Compare);

            foreach (MethodInfo info in from info in methods let handler = info.GetCustomAttributes(false).OfType<EventHandler>().FirstOrDefault() where handler != null where !(@event is ICancellableEvent) || !((ICancellableEvent)@event).IsCancelled || handler.IgnoreCancelled select info)
            {
                object instance = null;
                try
                {
                    foreach (var c in _listenerMethods.Keys.Where(c => _listenerMethods.ContainsKey(c) && _listenerMethods[c].Contains(info)))
                    {
                        instance = c;
                    }
                }
                catch (KeyNotFoundException e)
                {
                    return;
                }


                Action action = delegate
                {
                    info.Invoke(instance, BindingFlags.InvokeMethod, null, new object[] { @event },
                        CultureInfo.CurrentCulture);
                };

                if (@event.IsAsync)
                {
                    Utils.TaskDispatcher.Instance.QueueAsync(action);
                }
                else
                {
                    Utils.TaskDispatcher.Instance.QueueUpdateFixed(action);
                }
            }
        }

        public void ClearListeners(IRocketPlugin plugin)
        {
            if (!_listeners.ContainsKey(plugin)) return;
            foreach (IListener listener in _listeners[plugin])
            {
                if (_listenerMethods.ContainsKey(listener))
                {
                    _listenerMethods.Remove(listener);
                }
            }

            _listeners.Remove(plugin);
        }

        internal void Shutdown()
        {
            _instance = null;
        }
    }

    public class EventComprarer
    {
        public static int Compare(MethodInfo a, MethodInfo b)
        {
            EventPriority priorityA = a.GetCustomAttributes(false).OfType<EventHandler>().FirstOrDefault().Priority;
            EventPriority priorityB = b.GetCustomAttributes(false).OfType<EventHandler>().FirstOrDefault().Priority;

            if (priorityA > priorityB)
            {
                return 1;
            }

            if (priorityB > priorityA)
            {
                return -1;
            }

            return 0;
        }
    }
}