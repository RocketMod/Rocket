using Rocket.API;
using Rocket.API.Eventing;

namespace Rocket.Core.Implementation.Events
{
    public class ImplementationReadyEvent : Event
    {
        public ImplementationReadyEvent(IImplementation implementation) : this(implementation, true) { }

        public ImplementationReadyEvent(IImplementation implementation, bool global = true) : base(global)
        {
            Implementation = implementation;
        }

        public ImplementationReadyEvent(IImplementation implementation,
                                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                        bool global = true) : base(executionTarget, global)
        {
            Implementation = implementation;
        }

        public ImplementationReadyEvent(IImplementation implementation, string name = null,
                                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                        bool global = true) : base(name, executionTarget, global)
        {
            Implementation = implementation;
        }

        public IImplementation Implementation { get; }
    }
}