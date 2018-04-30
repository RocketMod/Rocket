using Rocket.API.Eventing;

namespace Rocket.API.Plugins
{
    /// <summary>
    ///     Represents a RocketMod plugin.
    /// </summary>
    public interface IPlugin : IEventEmitter
    {
        /// <summary>
        ///     The working directory of the plugin.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        ///     Activates the plugin.
        /// </summary>
        /// <returns><b>true</b> if the plugin could be activated; otherwise, <b>false</b>.</returns>
        bool Activate();

        /// <summary>
        ///     Deactivates the plugin.
        /// </summary>
        /// <returns><b>true</b> if the plugin could be deactivated; otherwise, <b>false</b>.</returns>
        bool Deactivate();

        /// <summary>
        ///     Reloads the plugin and all related components (e.g. configurations, translations, permissions...).
        /// </summary>
        void Reload();
    }
}