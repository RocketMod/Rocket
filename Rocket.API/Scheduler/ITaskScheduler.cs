using System;
using System.Collections.Generic;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Scheduler
{
    /// <summary>
    ///     The service responsible for scheduling and managing tasks.
    /// </summary>
    public interface ITaskScheduler: IService
    {
        /// <summary>
        ///     The current scheduled and ongoing tasks. Does not include finished and cancelled tasks.
        /// </summary>
        IEnumerable<ITask> Tasks { get; }

        /// <summary>
        ///     Schedules an action for all frames on the main thread. See <see cref="ExecutionTargetContext.EveryFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask ScheduleEveryFrame(ILifecycleObject owner, Action action, string taskName);

        /// <summary>
        ///     Schedules an action for the next frame on the main thread. See <see cref="ExecutionTargetContext.NextFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param> 
        /// <param name="taskName">The tasks human friendly name.</param> 
        ITask ScheduleNextFrame(ILifecycleObject owner, Action action, string taskName);

        /// <summary>
        ///     Schedules an action.
        /// </summary>
        /// <seealso cref="ExecutionTargetContext" />
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param>  
        /// <param name="taskName">The tasks human friendly name.</param>
        /// <param name="target">The target event execution context.</param>
        ITask ScheduleUpdate(ILifecycleObject owner, Action action, string taskName, ExecutionTargetContext target);

        /// <summary>
        ///     Schedule an action which includes physics interactions (e.g. applying force to an object) for the next physics
        ///     update. See <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask ScheduleNextPhysicUpdate(ILifecycleObject owner, Action action, string taskName);

        /// <summary>
        ///     Schedule an action which includes physics interaction (e.g. applying force to an object) for every physics update.
        ///     See <see cref="ExecutionTargetContext.EveryPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask ScheduleEveryPhysicUpdate(ILifecycleObject owner, Action action, string taskName);

        /// <summary>
        ///     Schedules an action for all frame on a separate thread. See <see cref="ExecutionTargetContext.EveryAsyncFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask ScheduleEveryAsyncFrame(ILifecycleObject owner, Action action, string taskName);

        /// <summary>
        ///     Schedules an action for the next frame on a separate thread. See
        ///     <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask ScheduleNextAsyncFrame(ILifecycleObject owner, Action action, string taskName);

        /// <summary>
        ///     Cancels a task. Tasks are automatically cancelled on a plugin unload.
        /// </summary>
        bool CancelTask(ITask task);

        /// <summary>
        ///     Executes the given task roughly after the delay.
        /// </summary>
        /// <param name="object">The owner of the task.</param>
        /// <param name="action">The action to schedule. Must not block thread if <i>runAsync</i> equals <b>false</b>.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="runAsync">Defines if the task should run in a separate thread.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask ScheduleDelayed(ILifecycleObject @object, Action action, string taskName, TimeSpan delay, bool runAsync = false);

        /// <summary>
        ///     Executes the given task roughly at the given time. Runs on the main thread if one exists. Note that after restarts the tasks may not be preserved.
        /// </summary>
        /// <param name="object">The owner of the task.</param>
        /// <param name="action">The action to schedule. Must not block thread if <i>runAsync</i> equals <b>false</b>.</param>
        /// <param name="date">The date to run the task at.</param>
        /// <param name="runAsync">Defines if the task should run in a separate thread.</param> 
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask ScheduleAt(ILifecycleObject @object, Action action, string taskName, DateTime date, bool runAsync = false);

        /// <summary>
        ///     Execute the given task roughly at the given period. Runs on the main thread if one exists.
        /// </summary>
        /// <param name="object">The owner of the task.</param>
        /// <param name="action">The action to schedule. Must not block thread if <i>runAsync</i> equals <b>false</b>.</param>
        /// <param name="period">The period of the task.</param>
        /// <param name="delay">The delay of the task (optional).</param>
        /// <param name="runAsync">Defines if the task should run in a separate thread.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        ITask SchedulePeriodically(ILifecycleObject @object, Action action, string taskName, TimeSpan period, TimeSpan? delay = null, bool runAsync = false);
    }
}