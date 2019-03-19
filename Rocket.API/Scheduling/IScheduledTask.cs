using System;
using System.Threading.Tasks;

namespace Rocket.API.Scheduling
{
    /// <summary>
    ///     Represents a scheduled task.
    /// </summary>
    public interface IScheduledTask
    {
        /// <summary>
        ///     Gets the tasks ID.
        /// </summary>
        int TaskId { get; }

        /// <summary>
        ///     The task name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The period to execute the task in.
        /// </summary>
        TimeSpan? Period { get; }

        /// <summary>
        ///     The time the task has started or will start.
        /// </summary>
        DateTime? StartTime { get; } 

        /// <summary>
        ///     The time the task will end or has end.
        /// </summary>
        DateTime? EndTime { get; }

        /// <summary>
        ///     The last time the task was run.
        /// </summary>
        DateTime? LastRunTime { get; }

        /// <summary>
        ///     The owner of the task.
        /// </summary>
        ILifecycleObject Owner { get; }

        /// <summary>
        ///     The action to execute.
        /// </summary>
        Action Action { get; }

        /// <summary>
        ///     Checks if the task was cancelled.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        ///     Defines how and when the task should be executed.
        /// </summary>
        /// <seealso cref="ExecutionTargetContext" />
        ExecutionTargetContext ExecutionTarget { get; }

        /// <summary>
        ///     Checks if the task has finished.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        ///     The parent task scheduler.
        /// </summary>
        ITaskScheduler Scheduler { get; }
    }
}