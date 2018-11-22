using System;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Scheduling;

namespace Rocket.Console.Scheduling
{
    public class SimpleTask : ITask
    {
        private readonly Core.Util.WeakReference<ILifecycleObject> ownerRef;

        internal SimpleTask(int taskId, string taskName, ITaskScheduler scheduler, ILifecycleObject owner, Task action,
                            ExecutionTargetContext executionTarget)
        {
            Name = taskName;
            ownerRef = new Core.Util.WeakReference<ILifecycleObject>(owner);
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

        public bool IsReferenceAlive => ownerRef.IsAlive;

        public ILifecycleObject Owner
        {
            get
            {
                if (ownerRef.IsAlive)
                    return ownerRef.Target;

                throw new ObjectDisposedException("The owner has been collected by the GC.");
            }
        }

        public Task Action { get; }
        public bool IsCancelled { get; internal set; }
        public ExecutionTargetContext ExecutionTarget { get; }
        public bool IsFinished => !Scheduler.Tasks.Contains(this);
        public ITaskScheduler Scheduler { get; }
    }
}