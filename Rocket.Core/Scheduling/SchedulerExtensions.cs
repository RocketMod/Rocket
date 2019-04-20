using Rocket.API;
using Rocket.API.Scheduling;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Core.Scheduling
{
    public static class SchedulerExtensions
    {
        public delegate Task AsyncAction();

        /// <inheritdoc cref="ITaskScheduler.ScheduleUpdate"/>
        public static IScheduledTask ScheduleTaskUpdate(this ITaskScheduler taskScheduler, ILifecycleObject @owner,
                                            AsyncAction action, string taskName, ExecutionTargetContext frame)
        {
            return taskScheduler.ScheduleUpdate(@owner, () => AsyncHelper.RunSync(() => action()), taskName, frame);
        }

        /// <inheritdoc cref="ITaskScheduler.ScheduleAt"/>
        public static IScheduledTask ScheduleTaskAt(this ITaskScheduler taskScheduler, ILifecycleObject @owner,
                                            AsyncAction action, string taskName, DateTime date, bool runAsync = false)
        {
            return taskScheduler.ScheduleAt(@owner, () => AsyncHelper.RunSync(() => action()), taskName, date, runAsync);
        }

        /// <inheritdoc cref="ITaskScheduler.SchedulePeriodically"/>
        public static IScheduledTask ScheduleTaskPeriodically(this ITaskScheduler taskScheduler, ILifecycleObject @owner,
                                            AsyncAction action, string taskName, TimeSpan period, TimeSpan? delay = null, bool runAsync = false)
        {
            return taskScheduler.SchedulePeriodically(@owner, () => AsyncHelper.RunSync(() => action()), taskName, period, delay, runAsync);
        }

        /// <summary>
        ///     Schedules an action for the next frame on the main thread. See <see cref="ExecutionTargetContext.NextFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param> 
        /// <param name="taskName">The tasks human friendly name.</param> 
        public static IScheduledTask ScheduleNextFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.NextFrame);

        /// <inheritdoc cref="ScheduleNextFrame(ITaskScheduler, ILifecycleObject, Action, string)"/>
        public static IScheduledTask ScheduleTaskNextFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, AsyncAction action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, () => AsyncHelper.RunSync(() => action()), taskName, ExecutionTargetContext.NextFrame);

        /// <summary>
        ///     Schedules an action for all frames on the main thread. See <see cref="ExecutionTargetContext.EveryFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static IScheduledTask ScheduleEveryFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.EveryFrame);

        /// <inheritdoc cref="ScheduleEveryFrame"/>
        public static IScheduledTask ScheduleTaskEveryFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, AsyncAction action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, () => AsyncHelper.RunSync(() => action()), taskName, ExecutionTargetContext.EveryFrame);

        /// <summary>
        ///     Schedule an action which includes physics interactions (e.g. applying force to an object) for the next physics
        ///     update. See <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static IScheduledTask ScheduleNextPhysicUpdate(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.NextPhysicsUpdate);

        /// <inheritdoc cref="ScheduleNextPhysicUpdate"/>
        public static IScheduledTask ScheduleTaskNextPhysicUpdate(this ITaskScheduler taskScheduler, ILifecycleObject @owner, AsyncAction action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, () => AsyncHelper.RunSync(() => action()), taskName, ExecutionTargetContext.NextPhysicsUpdate);

        /// <summary>
        ///     Schedule an action which includes physics interaction (e.g. applying force to an object) for every physics update.
        ///     See <see cref="ExecutionTargetContext.EveryPhysicsUpdate" />.<br />
        ///     Execution time depends on the engine implemention.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static IScheduledTask ScheduleEveryPhysicUpdate(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.EveryPhysicsUpdate);

        /// <inheritdoc cref="ScheduleEveryPhysicUpdate"/>
        public static IScheduledTask ScheduleTaskEveryPhysicUpdate(this ITaskScheduler taskScheduler, ILifecycleObject @owner, AsyncAction action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, () => AsyncHelper.RunSync(() => action()), taskName, ExecutionTargetContext.EveryPhysicsUpdate);

        /// <summary>
        ///     Schedules an action for all frame on a separate thread. See <see cref="ExecutionTargetContext.EveryAsyncFrame" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static IScheduledTask ScheduleNextAsyncFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, Action action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, action, taskName, ExecutionTargetContext.NextAsyncFrame);

        /// <inheritdoc cref="ScheduleNextAsyncFrame"/>
        public static IScheduledTask ScheduleTaskNextAsyncFrame(this ITaskScheduler taskScheduler, ILifecycleObject @owner, AsyncAction action, string taskName)
            => taskScheduler.ScheduleUpdate(@owner, () => AsyncHelper.RunSync(() => action()), taskName, ExecutionTargetContext.NextAsyncFrame);

        /// <summary>
        ///     Schedules an action for the next frame on a separate thread. See
        ///     <see cref="ExecutionTargetContext.NextPhysicsUpdate" />.
        /// </summary>
        /// <param name="action">The action to schedule. Must not block the thread.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static IScheduledTask ScheduleEveryAsyncFrame(this ITaskScheduler taskScheduler, ILifecycleObject owner, Action action, string taskName)
            => taskScheduler.ScheduleUpdate(owner, action, taskName, ExecutionTargetContext.EveryAsyncFrame);

        /// <inheritdoc cref="ScheduleEveryAsyncFrame"/>
        public static IScheduledTask ScheduleTaskEveryAsyncFrame(this ITaskScheduler taskScheduler, ILifecycleObject owner, AsyncAction action, string taskName)
            => taskScheduler.ScheduleUpdate(owner, () => AsyncHelper.RunSync(() => action()), taskName, ExecutionTargetContext.EveryAsyncFrame);

        /// <summary>
        ///     Executes the given task roughly after the given delay.
        /// </summary>
        /// <param name="object">The owner of the task.</param>
        /// <param name="action">The action to schedule. Must not block thread if <i>runAsync</i> equals <b>false</b>.</param>
        /// <param name="taskScheduler">the Task Scheduler</param>
        /// <param name="delay">The delay.</param>
        /// <param name="runAsync">Defines if the task should run in a separate thread.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static IScheduledTask ScheduleDelayed(this ITaskScheduler taskScheduler, ILifecycleObject @object, Action action, string taskName, TimeSpan delay, bool runAsync = false)
            => taskScheduler.ScheduleAt(@object, action, taskName, DateTime.UtcNow + delay, runAsync);

        /// <inheritdoc cref="ScheduleDelayed"/>
        public static IScheduledTask ScheduleTaskDelayed(this ITaskScheduler taskScheduler, ILifecycleObject @object, AsyncAction action, string taskName, TimeSpan delay, bool runAsync = false)
            => taskScheduler.ScheduleAt(@object, () => AsyncHelper.RunSync(() => action()), taskName, DateTime.UtcNow + delay, runAsync);

        /// <summary>
        /// Schedules a task to run on main thread. If this is called from main thread, the action will run immediately.
        /// </summary>
        /// <param name="taskScheduler">The task scheduler.</param>
        /// <param name="owner">The owner of the task.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="taskName">The tasks human friendly name.</param>
        public static IScheduledTask RunOnMainThread(this ITaskScheduler taskScheduler, ILifecycleObject owner, Action action, string taskName)
        {
            if (Thread.CurrentThread == taskScheduler.MainThread)
            {
                action();
                return null;
            }

            return taskScheduler.ScheduleUpdate(owner, action, taskName, ExecutionTargetContext.NextFrame);
        }

        /// <inheritdoc cref="RunOnMainThread"/>
        public static IScheduledTask RunTaskOnMainThread(this ITaskScheduler taskScheduler, ILifecycleObject owner, AsyncAction action, string taskName)
        {
            return taskScheduler.RunOnMainThread(owner, action().GetAwaiter().GetResult, taskName);
        }
    }
}