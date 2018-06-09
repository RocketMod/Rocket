using Rocket.API;
using Rocket.API.Eventing;
using Rocket.Core.Eventing;

namespace Rocket.Core.Implementation.Events
{
    public class ImplementationReadyEvent : Event
    {
        public ImplementationReadyEvent(IHost host) : this(host, true) { }

        public ImplementationReadyEvent(IHost host, bool global = true) : base(global)
        {
            Host = host;
        }

        public ImplementationReadyEvent(IHost host,
                                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                        bool global = true) : base(executionTarget, global)
        {
            Host = host;
        }

        public IHost Host { get; }
    }
}