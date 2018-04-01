using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API;
using Rocket.API.Eventing;
using Rocket.API.Scheduler;

namespace Rocket.ConsoleImplementation
{
    public class SimpleTaskScheduler : ITaskScheduler
    {
        private readonly IEventManager _eventManager;

        public SimpleTaskScheduler(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public ReadOnlyCollection<ITask> Tasks => _tasks.AsReadOnly();
        private readonly List<ITask> _tasks = new List<ITask>();

        public ITask ScheduleEveryFrame(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleNextFrame(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask Schedule(ILifecycleObject @object, Action action, ExecutionTargetContext target)
        {
            var task = new SimpleTask(this, @object, action, target);

            TriggerEvent(task, (@event) =>
            {
                if (target != ExecutionTargetContext.Sync && @object.IsAlive)
                    return;

                if (((ICancellableEvent)@event).IsCancelled)
                    return;

                action();
                _tasks.Remove(task);
            });
            
            return task;
        }

        public ITask ScheduleNextPhysicUpdate(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextPhysicsUpdate);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleEveryPhysicUpdate(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryPhysicsUpdate);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleEveryAsyncFrame(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryAsyncFrame);
            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleNextAsyncFrame(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextAsyncFrame);
            TriggerEvent(task);
            return task;
        }

        private TaskScheduleEvent TriggerEvent(SimpleTask task, EventExecutedCallback cb = null)
        {
            var e = new TaskScheduleEvent(task);

            _eventManager.Emit(task.Owner, e, (@event) =>
            {
                task.IsCancelled = e.IsCancelled;

                if (!e.IsCancelled)
                    _tasks.Add(task);

                cb?.Invoke(@event);
            });

            return e;
        }

        public bool CancelTask(ITask t)
        {
            var task = (SimpleTask)t;
            task.IsCancelled = true;
            return _tasks.Remove(task);
        }
    }
}