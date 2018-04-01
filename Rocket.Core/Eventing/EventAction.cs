using System;
using System.Reflection;
using Rocket.API;
using Rocket.API.Eventing;
using EventHandler = Rocket.API.Eventing.EventHandler;

namespace Rocket.Core.Eventing
{
    class EventAction
    {
        public EventAction(
            ILifecycleObject owner, 
            Action<IEventEmitter, IEvent> action, 
            EventHandler handler, 
            string eventName,
            string emitterName)
        {
            Owner = owner;
            Action = action;
            Handler = handler;
            TargetEventType = eventName;
            EmitterName = emitterName;
        }
        
        public EventAction(
            ILifecycleObject owner,
            IEventListener listener,
            MethodInfo method,
            EventHandler handler, 
            string emitterName)
        {
            Owner = owner;
            Listener = listener;
            Method = method;
            Handler = handler;

            if (method.GetParameters().Length != 1 || !typeof(IEvent).IsAssignableFrom(method.GetParameters()[0].ParameterType))
                throw new Exception("Method: " + method.Name + " in type " + method.DeclaringType.FullName + " does not have correct signature for events!");
            Type targetType = method.GetParameters()[0].ParameterType;
            TargetEventType = targetType.Name.Replace("Event", "");
            EmitterName = emitterName;
        }
        
        public ILifecycleObject Owner { get; set; }

        public Action<IEventEmitter, IEvent> Action { get; }

        public MethodInfo Method { get; }

        public EventHandler Handler { get; }

        public IEventListener Listener { get; }

        public string TargetEventType { get; }
        public string EmitterName { get; }

        public void Invoke(IEventEmitter owner, IEvent @event)
        {
            Action?.Invoke(owner, @event);
            Method?.Invoke(owner, new object[] { @event });
        }
    }
}