using Rocket.API;
using Rocket.API.Eventing;

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

        public ImplementationReadyEvent(IHost host, string name = null,
                                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                        bool global = true) : base(name, executionTarget, global)
        {
            Host = host;
        }

        public IHost Host { get; }
    }
}