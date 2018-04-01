using System;

namespace Rocket.API.Scheduler
{
    public interface ITaskScheduler
    {
        /// <summary>
        /// Schedules a single action for all frames
        /// </summary>
        Task ScheduleEveryFrame(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame
        /// </summary>
        Task ScheduleNextFrame(ILifecycleObject @object, Action action);

        Task ScheduleAction(ILifecycleObject @object, Action action, ExecutionTargetContext target);
        
        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs every frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        Task SchedulePhysicUpdates(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs one time on next frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        Task SchedulePhysicUpdate(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedules a single action for all frame asynchronously
        /// </summary>
        Task AsyncScheduleEveryFrame(ILifecycleObject @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame asynchronously
        /// </summary>
        Task AsyncScheduleNextFrame(ILifecycleObject @object, Action action);
    }
}