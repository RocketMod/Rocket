using System;

namespace Rocket.Core.Eventing
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
    public class EventNameAttribute : Attribute
    {
        public string EventName { get; }

        public EventNameAttribute(string eventName)
        {
            EventName = eventName;
        }   
    }
}