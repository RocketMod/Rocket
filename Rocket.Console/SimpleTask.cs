using System;
using System.Linq;
using Rocket.API;
using Rocket.API.Scheduler;

namespace Rocket.Console
{
    public class SimpleTask : ITask
    {
        internal SimpleTask(ITaskScheduler scheduler, ILifecycleObject owner, Action action,
                            ExecutionTargetContext executionTarget)
        {
            Owner = owner;
            Action = action;
            ExecutionTarget = executionTarget;
            Scheduler = scheduler;
        }

        public string Name => null;
        public TimeSpan? Period => null;
        public DateTime? StartTime => null;
        public DateTime? EndTime => null;
        public ILifecycleObject Owner { get; }
        public Action Action { get; }
        public bool IsCancelled { get; internal set; }
        public ExecutionTargetContext ExecutionTarget { get; }
        public bool IsFinished => !Scheduler.Tasks.Contains(this);
        public ITaskScheduler Scheduler { get; }
    }
}