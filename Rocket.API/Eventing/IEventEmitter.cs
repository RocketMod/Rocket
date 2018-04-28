namespace Rocket.API.Eventing
{
    /// <summary>
    ///     An object that emits an event.
    /// </summary>
    public interface IEventEmitter : ILifecycleObject
    {
        /// <summary>
        ///     The name of the emitter. This is the name the listeners can subscribe to.
        /// </summary>
        string Name { get; }
    }
}