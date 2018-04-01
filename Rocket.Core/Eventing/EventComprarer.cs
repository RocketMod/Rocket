using System;
using System.Linq;
using System.Reflection;
using Rocket.API.Eventing;
using EventHandler = Rocket.API.Eventing.EventHandler;

namespace Rocket.Core.Eventing
{
    public class EventComprarer
    {
        public static int Compare(EventAction a, EventAction b)
        {
            EventPriority priorityA = a.Handler.Priority;
            EventPriority priorityB = b.Handler.Priority;

            return priorityA.CompareTo(priorityB);
        }
    }
}