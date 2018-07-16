using System;
using System.Collections.Generic;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Scheduling
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
        ///     Schedules an action.
        /// </summary>
        /// <seealso cref="ExecutionTargetContext" />
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="owner">The owner of the task.</param>  
        /// <param name="taskName">The tasks human friendly name.</param>
        /// <param name="target">The target event execution context.</param>
        ITask ScheduleUpdate(ILifecycleObject owner, Action action, string taskName, ExecutionTargetContext target);

        /// <summary>
        ///     Cancels a task. Tasks are automatically cancelled on a plugin unload.
        /// </summary>
        bool CancelTask(ITask task);

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