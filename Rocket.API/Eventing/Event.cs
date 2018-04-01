using Rocket.API.Scheduler;

namespace Rocket.API.Eventing
{
    public class Event : IEvent
    {
        protected Event() : this(null)
        {

        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        protected Event(EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync) : this(null, executionTarget)
        {
            
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        /// <param name="name">The name of the event. Will be auto set when null.</param>
        protected Event(string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync)
        {
            Name = name ?? GetType().Name.Replace("Event", "");
            ExecutionTarget = executionTarget;
        }

        public event EventExecutedCallback OnEventExecuted;
        public ILifecycleObject Sender { get; }
        public string Name { get; }
        public EventExecutionTargetContext ExecutionTarget { get; }
    }
}
