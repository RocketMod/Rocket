using System;
using Rocket.API.Scheduling;

namespace Rocket.Core.Scheduling
{
    public class SyncTaskRunner : ITaskRunner
    {
        public string ServiceName { get; } = "Sync Task Runner";

        public bool SupportsTask(IScheduledTask task)
        {
            return task.ExecutionTarget != ExecutionTargetContext.Async
                && task.ExecutionTarget != ExecutionTargetContext.EveryAsyncFrame
                && task.ExecutionTarget != ExecutionTargetContext.NextAsyncFrame;
        }

        public void Run(IScheduledTask task, Action completedAction, out Exception taskException)
        {
            try
            {
                task.Action();
            }
            catch(Exception ex)
            {
                taskException = ex;
                return;
            }

            completedAction();
        }
    }
}