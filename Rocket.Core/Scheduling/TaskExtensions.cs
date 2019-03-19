using Rocket.API.Scheduling;

namespace Rocket.Core.Scheduling
{
    public static class TaskExtensions
    {
        public static bool Cancel(this IScheduledTask task)
        {
            return task.Scheduler.CancelTask(task);
        }
    }
}