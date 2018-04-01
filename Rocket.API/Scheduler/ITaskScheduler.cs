using System;
using System.Collections.ObjectModel;

namespace Rocket.API.Scheduler
{
    public interface ITaskScheduler
    {
        /// <summary>
        /// Current registered tasks
        /// </summary>
        ReadOnlyCollection<ITask> Tasks { get; }

        /// <summary>
        /// Schedules a single action for all frames
        /// </summary>
        ITask ScheduleEveryFrame(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame
        /// </summary>
        ITask ScheduleNextFrame(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedule an action
        /// </summary>
        ITask Schedule(ILifecycleObject @object, Action action, ExecutionTargetContext target);

        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs every frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        ITask ScheduleNextPhysicUpdate(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs one time on next frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        ITask ScheduleEveryPhysicUpdate(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedules a single action for all frame asynchronously
        /// </summary>
        ITask ScheduleEveryAsyncFrame(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame asynchronously
        /// </summary>
        ITask ScheduleNextAsyncFrame(ILifecycleObject @object, Action action);

        /// <summary>
        /// Cancel a task
        /// </summary>
        bool CancelTask(ITask task);
    }
}