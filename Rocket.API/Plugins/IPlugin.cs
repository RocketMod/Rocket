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
        IPluginManager PluginManager { get; }

        /// <summary>
        ///     Activates the plugin.
        /// </summary>
        /// <returns><b>true</b> if the plugin could be activated; otherwise, <b>false</b>.</returns>
        bool Load(bool isReload);

        /// <summary>
        ///     Deactivates the plugin.
        /// </summary>
        /// <returns><b>true</b> if the plugin could be deactivated; otherwise, <b>false</b>.</returns>
        bool Unload();

        /// <summary>
        ///     The dependency container;
        /// </summary>
        IDependencyContainer Container { get; }
    }
}