using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.API;
using Rocket.API.Eventing;

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
            Owner = owner;
            Action = action;
            Handler = handler;
            TargetEventNames = eventNames;
        }

        public EventAction(ILifecycleObject owner,
                           IEventListener listener,
                           MethodInfo method,
                           EventHandler handler, Type type)
        {
            Owner = owner;
            Listener = listener;
            Action = (sender, @event) => method.Invoke(listener, new object[] {sender, @event});
            Handler = handler;
            TargetEventNames = EventBus.GetEventNames(type);
            TargetEventType = type;
        }

        public EventAction(ILifecycleObject owner, EventCallback action, EventHandler handler, Type eventType)
        {
            Owner = owner;
            Action = action;
            Handler = handler;
            TargetEventNames = EventBus.GetEventNames(eventType);
            TargetEventType = eventType;
        }

        public Type TargetEventType { get; set; }

        public ILifecycleObject Owner { get; set; }

        public EventCallback Action { get; }

        public EventHandler Handler { get; }

        public IEventListener Listener { get; }

        public List<string> TargetEventNames { get; }
    }
}