using Rocket.API.Eventing;

namespace Rocket.API.Scheduler
{
    public class TaskScheduleEvent : Event, ICancellableEvent
    {
        public ITask Task { get; }

        public TaskScheduleEvent(ITask task) : base(EventExecutionTargetContext.Sync)
        {
            Task = task;
        }

        public bool IsCancelled { get; }
    }
}