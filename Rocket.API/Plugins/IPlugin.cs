using System.Threading.Tasks;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;

namespace Rocket.API.Plugins
{
    /// <summary>
    ///     Represents a RocketMod plugin.
    /// </summary>
    public interface IPlugin : IEventEmitter, IConfigurationContext
    {
        /// <summary>
        ///     The related plugin manager.
        /// </summary>
        IPluginLoader PluginLoader { get; }

        /// <summary>
        ///     The dependency container;
        /// </summary>
        IDependencyContainer Container { get; }

        /// <summary>
        ///     Activates the plugin.
        /// </summary>
        /// <returns><b>true</b> if the plugin could be activated; otherwise, <b>false</b>.</returns>
        Task<bool> ActivateAsync(bool isReload);

        /// <summary>
        ///     Deactivates the plugin.
        /// </summary>
        /// <returns><b>true</b> if the plugin could be deactivated; otherwise, <b>false</b>.</returns>
        Task<bool> DeactivateAsync();
    }
}