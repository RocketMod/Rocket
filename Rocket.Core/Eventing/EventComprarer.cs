using Rocket.API.Eventing;

namespace Rocket.Core.Eventing
{
    class EventComprarer
    {
        public static int Compare(EventAction a, EventAction b)
        {
            EventPriority priorityA = a.Handler.Priority;
            EventPriority priorityB = b.Handler.Priority;

            return priorityA.CompareTo(priorityB);
        }
    }
}