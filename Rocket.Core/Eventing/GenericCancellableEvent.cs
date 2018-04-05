using Rocket.API.Eventing;

namespace Rocket.Core.Eventing
{
    public class GenericCancellableEvent : GenericEvent, ICancellableEvent
    {
        public GenericCancellableEvent(string name, EventExecutionTargetContext ctx = EventExecutionTargetContext.Sync)
            : base(name, ctx) { }

        public bool IsCancelled { get; set; }
    }
}