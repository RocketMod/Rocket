using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.API;
using Rocket.API.Eventing;
using Rocket.Core.Extensions;

namespace Rocket.Core.Eventing
{
    internal class EventAction
    {
        public EventAction(
            ILifecycleObject owner,
            EventCallback action,
            EventHandler handler,
            List<string> eventNames)
        {
            Owner = new Util.WeakReference<ILifecycleObject>(owner);
            Action = action;
            Handler = handler;
            TargetEventNames = eventNames;
        }

        public EventAction(ILifecycleObject owner,
                           IEventListener listener,
                           MethodInfo method,
                           EventHandler handler, Type type)
        {
            Owner = new Util.WeakReference<ILifecycleObject>(owner);
            Listener = listener;
            Action = (sender, @event) => method.InvokeWithTaskSupport(listener, new object[] {sender, @event});
            Handler = handler;
            TargetEventNames = EventBus.GetEventNames(type);
            TargetEventType = type;
        }

        public EventAction(ILifecycleObject owner, EventCallback action, EventHandler handler, Type eventType)
        {
            Owner = new Util.WeakReference<ILifecycleObject>(owner);
            Action = action;
            Handler = handler;
            TargetEventNames = EventBus.GetEventNames(eventType);
            TargetEventType = eventType;
        }

        public Type TargetEventType { get; set; }

        public Util.WeakReference<ILifecycleObject> Owner { get; set; }

        public EventCallback Action { get; }

        public EventHandler Handler { get; }

        public IEventListener Listener { get; }

        public List<string> TargetEventNames { get; }
    }
}