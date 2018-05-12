namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     Defines a service which can be proxied.
    /// </summary>
    public interface IProxyableService : IService
    {
        /// <summary>
        ///     The unique name of the service implementation.
        /// </summary>
        string ServiceName { get; }
    }
}