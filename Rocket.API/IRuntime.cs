using System;
using System.Threading.Tasks;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;

namespace Rocket.API
{
    /// <summary>
    ///     Defines a RocketMod Runtime. The runtime is responsibe for initializing RocketMod itself.
    /// </summary>
    public interface IRuntime : IEventEmitter, IConfigurationContext, IService
    {
        /// <summary>
        ///     The base dependency container.
        /// </summary>
        IDependencyContainer Container { get; }

        /// <summary>
        ///     Initializes the runtime.
        /// </summary>
        /// <returns></returns>
        Task InitAsync();

        /// <summary>
        ///     Shuts down RocketMod and disposes all services.
        /// </summary>
        Task ShutdownAsync();

        /// <summary>
        ///     The RocketMod version.
        /// </summary>
        Version Version { get; }
    }
}