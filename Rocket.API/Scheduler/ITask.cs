using System;

namespace Rocket.API.Scheduler
{
    /// <summary>
    ///     Represents a scheduled task.
    /// </summary>
    public interface ITask
    {
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
        /// <seealso cref="ExecutionTargetContext"/>
        ExecutionTargetContext ExecutionTarget { get; }

        /// <summary>
        ///     Checks if the task has finished.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        ///     Cancels the task.
        /// </summary>
        void Cancel();
    }
}