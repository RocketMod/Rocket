using Rocket.API.Eventing;
using Rocket.API;
using Rocket.API.Scheduler;

namespace Rocket.Core.Eventing
{
    public class Event : IEvent
    {
        protected Event() : this(null)
        {

        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        protected Event(ExecutionTargetContext executionTarget = ExecutionTargetContext.NextFrame) : this(null, executionTarget)
        {
            
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        /// <param name="name">The name of the event. Will be auto set when null.</param>
        protected Event(string name = null, ExecutionTargetContext executionTarget = ExecutionTargetContext.NextFrame)
        {
            Name = name ?? GetType().Name.Replace("Event", "");
            ExecutionTarget = executionTarget;
        }

        public ILifecycleObject Sender { get; }
        public string Name { get; }
        public ExecutionTargetContext ExecutionTarget { get; }
    }
}
