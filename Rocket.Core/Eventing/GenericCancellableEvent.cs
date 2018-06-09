using Rocket.API.Eventing;

namespace Rocket.Core.Eventing
{
    public class GenericCancellableEvent : GenericEvent, ICancellableEvent
    {
        public GenericCancellableEvent(EventExecutionTargetContext ctx = EventExecutionTargetContext.Sync)
            : base(ctx) { }

        public bool IsCancelled { get; set; }
    }
}