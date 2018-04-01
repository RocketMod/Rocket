using Rocket.API.Scheduler;

namespace Rocket.API.Eventing
{
    public interface IEvent : IEventArgs
    {
        /// <summary>
        /// Name of the event
        /// </summary>
        string Name { get; }

        /// <summary>
        /// True if the event should be fired async
        /// Notice: Some APIs do not work correctly async 
        /// </summary>
        EventExecutionTargetContext ExecutionTarget { get; }
    }
}