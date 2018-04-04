using Rocket.API.Eventing;

namespace Rocket.API.Scheduler {
    public class TaskScheduleEvent : Event, ICancellableEvent {
        public TaskScheduleEvent(ITask task) : base(EventExecutionTargetContext.Sync) {
            Task = task;
        }

        public ITask Task { get; }

        public bool IsCancelled { get; set; }
    }
}