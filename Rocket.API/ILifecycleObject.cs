namespace Rocket.API
{
    /// <summary>
    ///     Defines an object with a lifecycle.
    /// </summary>
    public interface ILifecycleObject
    {
        /// <summary>
        ///     Checks if the object is alive. When it is not alive, it must not be able to execute any code and should not be
        ///     notified of any events.
        /// </summary>
        bool IsAlive { get; }
    }
}