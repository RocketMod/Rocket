using System;

namespace Rocket.API.Scheduler
{
    public interface ITaskScheduler
    {
        /// <summary>
        /// Schedules a single action for all frames
        /// </summary>
        Task ScheduleEveryFrame(IRegisterableObject @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame
        /// </summary>
        Task ScheduleNextFrame(IRegisterableObject @object, Action action);

        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs every frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        Task SchedulePhysicUpdates(IRegisterableObject @object, Action action);

        /// <summary>
        /// Schedule an action which includes physics interactions
        /// (e.g. apply force to object).<br/>
        /// Runs one time on next frame.
        /// </summary>
        /// <param name="action">The action to schedule</param>
        Task SchedulePhysicUpdate(IRegisterableObject @object, Action action);

        /// <summary>
        /// Schedules a single action for all frame asynchronously
        /// </summary>
        Task AsyncScheduleEveryFrame(IRegisterableObject @object, Action action);

        /// <summary>
        /// Schedules a single action for the next frame asynchronously
        /// </summary>
        Task AsyncScheduleNextFrame(IRegisterableObject @object, Action action);
    }
}