using System;
using System.Threading;
using Rocket.API;
using Rocket.API.Scheduling;

namespace Rocket.Core.Scheduling
{
    public static class SchedulerExtensions
    {
        /// <summary>
        ///     Schedules an action for the next frame on the main thread. See <see cref="ExecutionTargetContext.NextFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param> 
        /// <param name="taskName">The tasks human friendly name.</param> 
        public static ITask ScheduleNextFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName) 
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.NextFrame);

        /// <summary>
        ///     Schedules an action for all frames on the main thread. See <see cref="ExecutionTargetContext.EveryFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static ITask ScheduleEveryFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName) 
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.EveryFrame);

        /// <summary>
        ///     Schedule an action which includes physics interactions (e.g. applying force to an object) for the next physics
        ///     update. See <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static ITask ScheduleNextPhysicUpdate(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName) 
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.NextPhysicsUpdate);

        /// <summary>
        ///     Schedule an action which includes physics interaction (e.g. applying force to an object) for every physics update.
        ///     See <see cref="ExecutionTargetContext.EveryPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static ITask ScheduleEveryPhysicUpdate(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName) 
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.EveryPhysicsUpdate);

        /// <summary>
        ///     Schedules an action for all frame on a separate thread. See <see cref="ExecutionTargetContext.EveryAsyncFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static ITask ScheduleNextAsyncFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName) 
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.NextAsyncFrame);

        /// <summary>
        ///     Schedules an action for the next frame on a separate thread. See
        ///     <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static ITask ScheduleEveryAsyncFrame(this ITaskScheduler taskScheduler, ILifecycleObject owner, Action action, string taskName) 
            => taskScheduler.ScheduleUpdate(owner, action, taskName, ExecutionTargetContext.EveryAsyncFrame);

        /// <summary>
        ///     Executes the given task roughly after the given delay.
        /// </summary>
        /// <param name="object">The owner of the task.</param>
        /// <param name="action">The action to schedule. Must not block thread if <i>runAsync</i> equals <b>false</b>.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="delay">The delay.</param>
        /// <param name="runAsync">Defines if the task should run in a separate thread.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static ITask ScheduleDelayed(this ITaskScheduler taskScheduler, ILifecycleObject @object, Action action, string taskName, TimeSpan delay, bool runAsync = false)
            => taskScheduler.ScheduleAt(@object, action, taskName, DateTime.UtcNow + delay, runAsync);

        public static ITask RunOnMainThread(this ITaskScheduler taskScheduler, ILifecycleObject owner, Action action, string taskName)
        {
            if (Thread.CurrentThread == taskScheduler.MainThread)
            {
                action();
                return null;
            }

            return taskScheduler.ScheduleUpdate(owner, action, taskName, ExecutionTargetContext.NextFrame);
        }
    }
}