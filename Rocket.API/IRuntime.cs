using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;

namespace Rocket.API
{
    /// <summary>
    ///     Defines a RocketMod Runtime. The runtime is responsibe for initializing RocketMod itself.
    /// </summary>
    public interface IRuntime : IEventEmitter, IConfigurationContext
    {
        /// <summary>
        ///     The base dependency container.
        /// </summary>
        IDependencyContainer Container { get; }
    }
}