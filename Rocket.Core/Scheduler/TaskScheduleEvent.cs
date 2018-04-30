using Rocket.API.Eventing;
using Rocket.API.Scheduler;

namespace Rocket.Core.Scheduler
{
    public class TaskScheduleEvent : Event, ICancellableEvent
    {
        public TaskScheduleEvent(ITask task) : base(EventExecutionTargetContext.Sync)
        {
            Task = task;
        }

        public ITask Task { get; }

        public bool IsCancelled { get; set; }
    }
}