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
            if(TriggerEvent(task))
                _tasks.Add(task);
            return task;
        }

        public ITask ScheduleNextFrame(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextFrame);
            if (TriggerEvent(task))
                _tasks.Add(task);
            return task;
        }

        public ITask Schedule(ILifecycleObject @object, Action action, ExecutionTargetContext target)
        {
            var task = new SimpleTask(this, @object, action, target);
            if (TriggerEvent(task))
                _tasks.Add(task);
            return task;
        }

        public ITask ScheduleNextPhysicUpdate(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextPhysicsUpdate);
            if (TriggerEvent(task))
                _tasks.Add(task);
            return task;
        }

        public ITask ScheduleEveryPhysicUpdate(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryPhysicsUpdate);
            if (TriggerEvent(task))
                _tasks.Add(task);
            return task;
        }

        public ITask ScheduleEveryAsyncFrame(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.EveryAsyncFrame);
            if (TriggerEvent(task))
                _tasks.Add(task);
            return task;
        }

        public ITask ScheduleNextAsyncFrame(ILifecycleObject @object, Action action)
        {
            var task = new SimpleTask(this, @object, action, ExecutionTargetContext.NextAsyncFrame);
            if (TriggerEvent(task))
                _tasks.Add(task);
            return task;
        }

        private bool TriggerEvent(ITask task)
        {
            var e = new TaskScheduleEvent(task);
            _eventManager.Emit(task.Owner, e);
            return !e.IsCancelled;
        }

        public bool CancelTask(ITask t)
        {
            var task = (SimpleTask) t;
            task.IsCancelled = true;
            return _tasks.Remove(task);
        }
    }
}