namespace Rocket.API.Event
{
    /// <summary>
    /// Makes an <see cref="Event"/> cancellable
    /// </summary>
    public interface ICancellableEvent
    {
        /// <summary>
        /// Should the event be cancelled? Also if this is set to true, listener methods which doesn't have <see cref="EventHandler.IgnoreCancelled"/> set to true won't receive the event anymore.
        /// </summary>
        bool IsCancelled { get; set; }
    }
}