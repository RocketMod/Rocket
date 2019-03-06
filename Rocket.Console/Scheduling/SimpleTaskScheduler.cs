using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Scheduling;
using Rocket.Core.Logging;
using Rocket.Core.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Console.Scheduling
{
    public class SimpleTaskScheduler : ITaskScheduler
    {
        protected IDependencyContainer Container { get; set; }
        protected List<SimpleTask> InternalTasks { get; set; }

        private volatile int taskIds;

        public SimpleTaskScheduler(IDependencyContainer container)
        {
            Container = container;
            MainThread = Thread.CurrentThread;

            (new AsyncThreadPool(this)).Start();
            InternalTasks = new List<SimpleTask>();
        }

        public Thread MainThread { get; }

        public IEnumerable<ITask> Tasks =>
            InternalTasks.Where(c => c.IsReferenceAlive && c.Owner.IsAlive);

        public ITask ScheduleUpdate(ILifecycleObject @object, Action action, string taskName, ExecutionTargetContext target)
        {
            if (!@object.IsAlive)
                return null;

            SimpleTask task = new SimpleTask(++taskIds, taskName, this, @object, action, target);

            TriggerEvent(task, async (sender, @event) =>
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
            if (!@object.IsAlive)
                return null;

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
            if (!@object.IsAlive)
                return null;

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

        protected internal virtual void RunTask(ITask t)
        {
            var task = (SimpleTask) t;
            if (!task.IsReferenceAlive)
            {
                InternalTasks.Remove(task);
                return;
            }

            if (!t.Owner.IsAlive)
                return;

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
                task.Action();
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
            InternalTasks.Remove((SimpleTask)task);
        }
    }
}