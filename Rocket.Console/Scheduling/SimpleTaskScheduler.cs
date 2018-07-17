using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Scheduling;
using Rocket.Core.Logging;
using Rocket.Core.Scheduling;

namespace Rocket.Console.Scheduling
{
    public class SimpleTaskScheduler : ITaskScheduler
    {
        protected IDependencyContainer Container { get; set; }
        protected List<ITask> InternalTasks { get; set; }

        private volatile int taskIds;

        public SimpleTaskScheduler(IDependencyContainer container)
        {
            Container = container;
            (new AsyncThreadPool(this)).Start();
            InternalTasks = new List<ITask>();
        }

        public IEnumerable<ITask> Tasks =>
            InternalTasks.Where(c => c.Owner.IsAlive);

        public ITask ScheduleUpdate(ILifecycleObject @object, Action action, string taskName, ExecutionTargetContext target)
        {
            SimpleTask task = new SimpleTask(++taskIds, taskName, this, @object, action, target);

            TriggerEvent(task, (sender, @event) =>
            {
                if (target != ExecutionTargetContext.Sync && @object.IsAlive) return;

                if (@event != null && ((ICancellableEvent)@event).IsCancelled) return;

                action();
                InternalTasks.Remove(task);
            });

            return task;
        }

        public ITask ScheduleAt(ILifecycleObject @object, Action action, string taskName, DateTime date, bool runAsync = false)
        {
            SimpleTask task = new SimpleTask(++taskIds, taskName, this, @object, action,
                runAsync ? ExecutionTargetContext.Async : ExecutionTargetContext.Sync)
            {
                StartTime = date
            };
            TriggerEvent(task);
            return task;
        }

        public ITask SchedulePeriodically(ILifecycleObject @object, Action action, string taskName, TimeSpan period, TimeSpan? delay = null,
                                          bool runAsync = false)
        {
            SimpleTask task = new SimpleTask(++taskIds, taskName, this, @object, action,
                runAsync ? ExecutionTargetContext.Async : ExecutionTargetContext.Sync)
            {
                Period = period
            };

            if (delay != null)
                task.StartTime = DateTime.Now + delay;

            TriggerEvent(task);
            return task;
        }

        public virtual bool CancelTask(ITask task)
        {
            if (task.IsFinished || task.IsCancelled)
                return false;

            ((SimpleTask)task).IsCancelled = true;
            return true;
        }

        protected virtual void TriggerEvent(SimpleTask task, EventCallback cb = null)
        {
            TaskScheduleEvent e = new TaskScheduleEvent(task);

            if (!(task.Owner is IEventEmitter owner)) return;

            IEventManager eventManager = Container.Resolve<IEventManager>();

            if (eventManager == null)
            {
                InternalTasks.Add(task);
                cb?.Invoke(owner, null);
                return;
            }

            eventManager.Emit(owner, e, @event =>
            {
                task.IsCancelled = e.IsCancelled;

                if (!e.IsCancelled)
                    InternalTasks.Add(task);

                cb?.Invoke(owner, @event);
            });
        }

        protected internal virtual void RunTask(ITask task)
        {
            if (task.StartTime != null && task.StartTime > DateTime.Now)
                return;

            if (task.EndTime != null && task.EndTime < DateTime.Now)
            {
                ((SimpleTask)task).EndTime = DateTime.Now;
                RemoveTask(task);
                return;
            }

            if (task.Period != null
                && ((SimpleTask)task).LastRunTime != null
                && DateTime.Now - ((SimpleTask)task).LastRunTime < task.Period)
                return;

            try
            {
                task.Action.Invoke();
                ((SimpleTask)task).LastRunTime = DateTime.Now;
            }
            catch (Exception e)
            {
                Container.Resolve<ILogger>().LogError("An exception occured in task: " + task.Name, e);
            }

            if (task.ExecutionTarget == ExecutionTargetContext.NextFrame
                || task.ExecutionTarget == ExecutionTargetContext.NextPhysicsUpdate
                || task.ExecutionTarget == ExecutionTargetContext.Async
                || task.ExecutionTarget == ExecutionTargetContext.NextAsyncFrame
                || task.ExecutionTarget == ExecutionTargetContext.Sync)
            {
                ((SimpleTask)task).EndTime = DateTime.Now;
                RemoveTask(task);
            }
        }

        protected virtual void RemoveTask(ITask task)
        {
            InternalTasks.Remove(task);
        }
    }
}