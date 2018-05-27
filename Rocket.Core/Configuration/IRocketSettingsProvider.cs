using Rocket.API.DependencyInjection;

namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     Provides general settings for RocketMod and its own services.
    /// </summary>
    public interface IRocketSettingsProvider: IService
    {
        /// <summary>
        ///     The RocketMod settings.
        /// </summary>
        RocketSettings Settings { get; }

        /// <summary>
        ///     Loads the settings.
        /// </summary>
        void Load();

        /// <summary>
        ///     Reloads the settings.
        /// </summary>
        void Reload();

        /// <summary>
        ///     Saves the settings.
        /// </summary>
        void Save();
    }
}