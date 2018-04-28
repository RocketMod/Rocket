namespace Rocket.API.Eventing
{
    /// <summary>
    ///     Defines when and where event listeners should be notified.
    /// </summary>
    public enum EventExecutionTargetContext
    {
        /// <summary>
        ///     Notifies the subscriptions on the next frame update.
        /// </summary>
        NextFrame,

        /// <summary>
        ///     Notifies the subscriptions on the next frame update from a separate thread.
        /// </summary>
        NextAsyncFrame,

        /// <summary>
        ///     Notifies the subscriptions the event on the next physics update.
        /// </summary>
        NextPhysicsUpdate,

        /// <summary>
        ///     Notifies the subscriptions the event immediately.
        /// </summary>
        Sync
    }
}