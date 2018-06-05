using Rocket.API.Scheduler;

namespace Rocket.Core.Scheduler
{
    public static class TaskExtensions
    {
        public static bool Cancel(this ITask task)
        {
            return task.Scheduler.CancelTask(task);
        }
    }
}