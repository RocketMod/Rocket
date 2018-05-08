namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     Provides general settings for RocketMod and its own services.
    /// </summary>
    public interface IRocketSettingsProvider
    {
        /// <summary>
        ///     The RocketMod settings.
        /// </summary>
        RocketSettings Settings { get; }
    }
}