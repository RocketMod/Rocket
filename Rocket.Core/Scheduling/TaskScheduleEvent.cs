using Rocket.API.Eventing;
using Rocket.API.Scheduling;
using Rocket.Core.Eventing;

namespace Rocket.Core.Scheduling
{
    public class TaskScheduleEvent : Event, ICancellableEvent
    {
        public TaskScheduleEvent(IScheduledTask task) : base(EventExecutionTargetContext.Sync)
        {
            Task = task;
        }

        public IScheduledTask Task { get; }

        public bool IsCancelled { get; set; }
    }
}