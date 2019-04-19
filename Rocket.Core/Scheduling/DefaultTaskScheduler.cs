using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Scheduling;
using Rocket.Core.Logging;

namespace Rocket.Core.Scheduling
{
    public class DefaultTaskScheduler : ITaskScheduler, IDisposable
    {
        private readonly ITaskRunner taskRunner;
        protected IDependencyContainer Container { get; set; }
        protected List<ScheduledTask> InternalTasks { get; set; }

        private volatile int taskIds;
        private readonly AsyncThreadPool asyncThreadPool;

        public DefaultTaskScheduler(IDependencyContainer container, ITaskRunner taskRunner)
        {
            this.taskRunner = taskRunner;
            Container = container;
            MainThread = Thread.CurrentThread;

            asyncThreadPool = new AsyncThreadPool(this);
            asyncThreadPool.Start();
            InternalTasks = new List<ScheduledTask>();
        }

        public Thread MainThread { get; }

        public IEnumerable<IScheduledTask> Tasks =>
            InternalTasks.Where(c => c.IsReferenceAlive && c.Owner.IsAlive);

        public IScheduledTask ScheduleUpdate(ILifecycleObject @object, Action action, string taskName, ExecutionTargetContext target)
        {
            if (!@object.IsAlive)
                return null;

            ScheduledTask task = new ScheduledTask(++taskIds, taskName, this, @object, action, target);

            TriggerEvent(task, async (sender, @event) =>
            {
                if (target != ExecutionTargetContext.Sync && @object.IsAlive) return;

                if (@event != null && ((ICancellableEvent)@event).IsCancelled) return;

                action();
                InternalTasks.Remove(task);
            });

            return task;
        }

        public IScheduledTask ScheduleAt(ILifecycleObject @object, Action action, string taskName, DateTime date, bool runAsync = false)
        {
            if (!@object.IsAlive)
                return null;

            ScheduledTask task = new ScheduledTask(++taskIds, taskName, this, @object, action,
                runAsync ? ExecutionTargetContext.Async : ExecutionTargetContext.Sync)
            {
                StartTime = date
            };
            TriggerEvent(task);
            return task;
        }

        public IScheduledTask SchedulePeriodically(ILifecycleObject @object, Action action, string taskName, TimeSpan period, TimeSpan? delay = null,
                                          bool runAsync = false)
        {
            if (!@object.IsAlive)
                return null;

            ScheduledTask task = new ScheduledTask(++taskIds, taskName, this, @object, action,
                runAsync ? ExecutionTargetContext.Async : ExecutionTargetContext.Sync)
            {
                Period = period
            };

            if (delay != null)
                task.StartTime = DateTime.UtcNow + delay;

            TriggerEvent(task);
            return task;
        }

        public virtual bool CancelTask(IScheduledTask task)
        {
            if (task.IsFinished || task.IsCancelled)
                return false;

            ((ScheduledTask)task).IsCancelled = true;
            return true;
        }

        protected virtual void TriggerEvent(ScheduledTask task, EventCallback cb = null)
        {
            asyncThreadPool.EventWaitHandle.Set();

            TaskScheduleEvent e = new TaskScheduleEvent(task);
            if (!(task.Owner is IEventEmitter owner))
            {
                return;
            }

            IEventBus eventBus = Container.Resolve<IEventBus>();
            if (eventBus == null)
            {
                InternalTasks.Add(task);
                cb?.Invoke(owner, null);
                return;
            }

            eventBus.Emit(owner, e, async @event =>
            {
                task.IsCancelled = e.IsCancelled;

                if (!e.IsCancelled)
                    InternalTasks.Add(task);

                cb?.Invoke(owner, @event);
            });
        }

        protected internal virtual void RunTask(IScheduledTask t)
        {
            var task = (ScheduledTask)t;
            if (!task.IsReferenceAlive)
            {
                InternalTasks.Remove(task);
                return;
            }

            if (!t.Owner.IsAlive)
            {
                return;
            }

            if (task.StartTime != null && task.StartTime > DateTime.UtcNow)
            {
                return;
            }

            if (task.EndTime != null && task.EndTime < DateTime.UtcNow)
            {
                task.EndTime = DateTime.UtcNow;
                RemoveTask(task);
                {
                    return;
                }
            }

            if (task.Period != null
                && task.LastRunTime != null
                && DateTime.UtcNow - task.LastRunTime < task.Period)
            {
                return;
            }

            taskRunner.Run(task, (taskException) =>
            {
                if (taskException != null)
                {
                    Container.Resolve<ILogger>().LogError("An exception occured in task: " + task.Name, taskException);
                    task.EndTime = DateTime.UtcNow;
                    RemoveTask(task);
                    return;
                }

                if (task.ExecutionTarget == ExecutionTargetContext.NextFrame
                    || task.ExecutionTarget == ExecutionTargetContext.NextPhysicsUpdate
                    || task.ExecutionTarget == ExecutionTargetContext.Async
                    || task.ExecutionTarget == ExecutionTargetContext.NextAsyncFrame
                    || task.ExecutionTarget == ExecutionTargetContext.Sync)
                {
                    task.EndTime = DateTime.UtcNow;
                    RemoveTask(task);
                }
            });
        }

        protected virtual void RemoveTask(IScheduledTask task)
        {
            InternalTasks.Remove((ScheduledTask)task);
        }

        public void Dispose()
        {
            asyncThreadPool.Stop();
        }
    }
}