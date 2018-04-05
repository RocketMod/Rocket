namespace Rocket.API.Scheduler
{
    public enum ExecutionTargetContext
    {
        /// <summary>
        ///     Execute action on the main thread on the next frame
        /// </summary>
        NextFrame,

        /// <summary>
        ///     Execute action asynchronously in a different thread
        /// </summary>
        NextAsyncFrame,

        /// <summary>
        ///     Execute action on next physic update
        /// </summary>
        NextPhysicsUpdate,

        /// <summary>
        ///     Execute action on current thread (will block current thread)
        /// </summary>
        Sync,
        EveryFrame,
        EveryAsyncFrame,
        EveryPhysicsUpdate
    }
}