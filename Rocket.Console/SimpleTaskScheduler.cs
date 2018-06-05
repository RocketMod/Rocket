using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Scheduler;
using Rocket.Core.Scheduler;

namespace Rocket.Console
{
    public class SimpleTaskScheduler : ITaskScheduler
    {
        private readonly IDependencyContainer container;
        private readonly List<ITask> tasks = new List<ITask>();

        public SimpleTaskScheduler(IDependencyContainer container)
        {
            this.container = container;
        }

        public IEnumerable<ITask> Tasks => tasks.AsReadOnly();

        public ITask ScheduleEveryFrame(ILifecycleObject @object, Action action, string taskName)
        {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleNextFrame(ILifecycleObject @object, Action action, string taskName)
        {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleUpdate(ILifecycleObject @object, Action action, string taskName, ExecutionTargetContext target)
        {
            SimpleTask task = new SimpleTask(this, @object, action, target);

            TriggerEvent(task, (sender, @event) =>
            {
                if (target != ExecutionTargetContext.Sync && @object.IsAlive) return;

                if (((ICancellableEvent) @event).IsCancelled) return;

                action();
                tasks.Remove(task);
            });

            return task;
        }

        public ITask ScheduleNextPhysicUpdate(ILifecycleObject @object, Action action, string taskName)
        {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextPhysicsUpdate);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleEveryPhysicUpdate(ILifecycleObject @object, Action action, string taskName)
        {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryPhysicsUpdate);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleEveryAsyncFrame(ILifecycleObject @object, Action action, string taskName)
        {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryAsyncFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleNextAsyncFrame(ILifecycleObject @object, Action action, string taskName)
        {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextAsyncFrame);
            TriggerEvent(task);
            return task;
        }

        public bool CancelTask(ITask t)
        {
            SimpleTask task = (SimpleTask) t;
            task.IsCancelled = true;
            return tasks.Remove(task);
        }

        public ITask ScheduleDelayed(ILifecycleObject @object, Action action, string taskName, TimeSpan delay, bool runAsync = false)
        {
            throw new NotImplementedException();
        }

        public ITask ScheduleAt(ILifecycleObject @object, Action action, string taskName, DateTime date, bool runAsync = false)
        {
            throw new NotImplementedException();
        }

        public ITask SchedulePeriodically(ILifecycleObject @object, Action action, string taskName, TimeSpan period, TimeSpan? delay = null,
                                          bool runAsync = false)
        {
            throw new NotImplementedException();
        }

        private void TriggerEvent(SimpleTask task, EventCallback cb = null)
        {
            TaskScheduleEvent e = new TaskScheduleEvent(task);

            if (!(task.Owner is IEventEmitter owner))
                return;

            if (!task.Owner.IsAlive)
                return;

            IEventManager eventManager = container.Resolve<IEventManager>();

            eventManager?.Emit(owner, e, @event =>
            {
                task.IsCancelled = e.IsCancelled;

                if (!e.IsCancelled) tasks.Add(task);

                cb?.Invoke(owner, @event);
            });
        }
    }
}