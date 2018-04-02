using Rocket.API.Scheduler;

namespace Rocket.API.Eventing
{
    public interface IEvent
    {
        /// <summary>
        /// Name of the event
        /// </summary>
        string Name { get; }

        bool Global { get; }

        /// <summary>
        /// True if the event should be fired async
        /// </summary>
        EventExecutionTargetContext ExecutionTarget { get; }
    }
}