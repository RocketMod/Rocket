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
        protected IDependencyContainer Container { get; set; }
        protected List<ScheduledTask> InternalTasks { get; set; }

        private volatile int taskIds;
        private readonly AsyncTaskRunner asyncTaskRunner;

        public DefaultTaskScheduler(IDependencyContainer container)
        {
            Container = container;
            MainThread = Thread.CurrentThread;

            asyncTaskRunner = new AsyncTaskRunner(this, ExecutionTargetSide.AsyncFrame);
            asyncTaskRunner.Start();
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

        public void RunFrameUpdate(ExecutionTargetSide target)
        {
            for (int index = 0; index < InternalTasks.Count; index++)
            {
                var task = InternalTasks[index];
                if (task.ExecutionTarget == ExecutionTargetContext.NextFrame
                    || task.ExecutionTarget == ExecutionTargetContext.EveryFrame)
                {
                    if (target != ExecutionTargetSide.SyncFrame)
                    {
                        continue;
                    }

                    RunTask(task);
                }

                if (task.ExecutionTarget == ExecutionTargetContext.NextPhysicsUpdate
                    || task.ExecutionTarget == ExecutionTargetContext.EveryPhysicsUpdate)
                {
                    if (target != ExecutionTargetSide.PhysicsFrame)
                    {
                        continue;
                    }

                    RunTask(task);
                }

                if (task.ExecutionTarget == ExecutionTargetContext.NextAsyncFrame
                    || task.ExecutionTarget == ExecutionTargetContext.EveryAsyncFrame)
                {
                    if (target != ExecutionTargetSide.AsyncFrame)
                    {
                        continue;
                    }

                    RunTask(task);
                }
            }
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
            if (task.ExecutionTarget == ExecutionTargetContext.Async
                || task.ExecutionTarget == ExecutionTargetContext.NextAsyncFrame
                || task.ExecutionTarget == ExecutionTargetContext.EveryAsyncFrame)
            {
                asyncTaskRunner.NotifyTaskScheduled();
            }

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

            try
            {

            }
            catch (Exception ex)
            {
                Container.Resolve<ILogger>().LogError("An exception occured in task: " + task.Name, ex);
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
        }

        protected virtual void RemoveTask(IScheduledTask task)
        {
            InternalTasks.Remove((ScheduledTask)task);
        }

        public void Dispose()
        {
            asyncTaskRunner?.Dispose();
        }
    }
}