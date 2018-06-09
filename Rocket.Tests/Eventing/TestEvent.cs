using Rocket.API.Eventing;
using Rocket.Core.Eventing;

namespace Rocket.Tests.Eventing
{
    public class TestEvent : Event, ICancellableEvent
    {
        public TestEvent() : this(true) { }

        public TestEvent(bool global = true) : base(global) { }

        public TestEvent(EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                         bool global = true) : base(executionTarget, global) { }

        public bool ValueChanged { get; set; }

        public bool IsCancelled { get; set; }
    }
}