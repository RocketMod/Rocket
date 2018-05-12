using System;
using System.Collections.ObjectModel;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Scheduler
{
    /// <summary>
    ///     The service responsible for scheduling and managing tasks.
    /// </summary>
    public interface ITaskScheduler: IService
    {
        /// <summary>
        ///     Gets all current scheduled tasks.
        /// </summary>
        ReadOnlyCollection<ITask> Tasks { get; }

        /// <summary>
        ///     Schedules an action for all frames on the main thread. See <see cref="ExecutionTargetContext.EveryFrame" />.
        /// </summary>
        ITask ScheduleEveryFrame(ILifecycleObject owner, Action action);

        /// <summary>
        ///     Schedules an action for the next frame on the main thread. See <see cref="ExecutionTargetContext.NextFrame" />.
        /// </summary>
        ITask ScheduleNextFrame(ILifecycleObject owner, Action action);

        /// <summary>
        ///     Schedules an action.
        /// </summary>
        /// <seealso cref="ExecutionTargetContext" />
        ITask Schedule(ILifecycleObject owner, Action action, ExecutionTargetContext target);

        /// <summary>
        ///     Schedule an action which includes physics interactions (e.g. applying force to an object) for the next physics
        ///     update. See <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        /// <param name="owner">The owner of the task.</param>
        ITask ScheduleNextPhysicUpdate(ILifecycleObject owner, Action action);

        /// <summary>
        ///     Schedule an action which includes physics interaction (e.g. applying force to an object) for every physics update.
        ///     See <see cref="ExecutionTargetContext.EveryPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        /// <param name="owner">The owner of the task.</param>
        ITask ScheduleEveryPhysicUpdate(ILifecycleObject owner, Action action);

        /// <summary>
        ///     Schedules an action for all frame on a separate thread. See <see cref="ExecutionTargetContext.EveryAsyncFrame" />.
        /// </summary>
        ITask ScheduleEveryAsyncFrame(ILifecycleObject @object, Action action);

        /// <summary>
        ///     Schedules an action for the next frame on a separate thread. See
        ///     <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.
        /// </summary>
        ITask ScheduleNextAsyncFrame(ILifecycleObject @object, Action action);

        /// <summary>
        ///     Cancels a task.
        /// </summary>
        bool CancelTask(ITask task);
    }
}