using System;

namespace Rocket.API.Scheduler
{
    public interface ITaskScheduler
    {
        /// <summary>
        /// Schedules a single action for all frames
        /// </summary>
        Task ScheduleEveryFrame(ILifecycleController @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame
        /// </summary>
        Task ScheduleNextFrame(ILifecycleController @object, Action action);

        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs every frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        Task SchedulePhysicUpdates(ILifecycleController @object, Action action);

        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs one time on next frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        Task SchedulePhysicUpdate(ILifecycleController @object, Action action);

        /// <summary>
        /// Schedules a single action for all frame asynchronously
        /// </summary>
        Task AsyncScheduleEveryFrame(ILifecycleController @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame asynchronously
        /// </summary>
        Task AsyncScheduleNextFrame(ILifecycleController @object, Action action);
    }
}