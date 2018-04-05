using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API;
using Rocket.API.Eventing;
using Rocket.API.Scheduler;

namespace Rocket.ConsoleImplementation {
    public class SimpleTaskScheduler : ITaskScheduler {
        private readonly IEventManager eventManager;
        private readonly List<ITask> tasks = new List<ITask>();

        public SimpleTaskScheduler(IEventManager eventManager) {
            this.eventManager = eventManager;
        }

        public ReadOnlyCollection<ITask> Tasks => tasks.AsReadOnly();

        public ITask ScheduleEveryFrame(ILifecycleObject @object, Action action) {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleNextFrame(ILifecycleObject @object, Action action) {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask Schedule(ILifecycleObject @object, Action action, ExecutionTargetContext target) {
            SimpleTask task = new SimpleTask(this, @object, action, target);

            TriggerEvent(task, (sender, @event) => {
                if (target != ExecutionTargetContext.Sync && @object.IsAlive)
                    return;

                if (((ICancellableEvent) @event).IsCancelled)
                    return;

                action();
                tasks.Remove(task);
            });

            return task;
        }

        public ITask ScheduleNextPhysicUpdate(ILifecycleObject @object, Action action) {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextPhysicsUpdate);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleEveryPhysicUpdate(ILifecycleObject @object, Action action) {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryPhysicsUpdate);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleEveryAsyncFrame(ILifecycleObject @object, Action action) {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryAsyncFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleNextAsyncFrame(ILifecycleObject @object, Action action) {
            SimpleTask task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextAsyncFrame);
            TriggerEvent(task);
            return task;
        }

        public bool CancelTask(ITask t) {
            SimpleTask task = (SimpleTask) t;
            task.IsCancelled = true;
            return tasks.Remove(task);
        }

        private void TriggerEvent(SimpleTask task, EventCallback cb = null) {
            TaskScheduleEvent e = new TaskScheduleEvent(task);

            if (!(task.Owner is IEventEmitter owner))
                return;

            eventManager.Emit(owner, e, @event => {
                task.IsCancelled = e.IsCancelled;

                if (!e.IsCancelled)
                    tasks.Add(task);

                cb?.Invoke(owner, @event);
            });
        }
    }
}