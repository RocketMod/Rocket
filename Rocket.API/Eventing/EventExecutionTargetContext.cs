namespace Rocket.API.Eventing
{
    public enum EventExecutionTargetContext
    {
        /// <summary>
        ///     Trigger the event on the next frame
        /// </summary>
        NextFrame,

        /// <summary>
        ///     Trigger the event on a separate thread on the next frame
        /// </summary>
        NextAsyncFrame,

        /// <summary>
        ///     Trigger the event on the next physics update
        /// </summary>
        NextPhysicsUpdate,

        /// <summary>
        ///     Trigger the event immediately
        /// </summary>
        Sync
    }
}