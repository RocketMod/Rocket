using System;
using System.Linq;
using Rocket.API;
using Rocket.API.Scheduler;

namespace Rocket.Console.Scheduling
{
    public class SimpleTask : ITask
    {
        internal SimpleTask(int taskId, string taskName, ITaskScheduler scheduler, ILifecycleObject owner, Action action,
                            ExecutionTargetContext executionTarget)
        {
            Name = taskName;
            Owner = owner;
            Action = action;
            ExecutionTarget = executionTarget;
            Scheduler = scheduler;
            TaskId = taskId;
        }
        
        public int TaskId { get; }
        public string Name { get; }

        public TimeSpan? Period { get; internal set; }
        public DateTime? StartTime { get; internal set; }
        public DateTime? EndTime { get; internal set; }
        public DateTime? LastRunTime { get; internal set; }
        public ILifecycleObject Owner { get; }

        public Action Action { get; }
        public bool IsCancelled { get; internal set; }
        public ExecutionTargetContext ExecutionTarget { get; }
        public bool IsFinished => !Scheduler.Tasks.Contains(this);
        public ITaskScheduler Scheduler { get; }
    }
}