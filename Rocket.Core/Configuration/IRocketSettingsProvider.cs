namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     Provides settings for RocketMod.
    /// </summary>
    public interface IRocketSettingsProvider
    {
        /// <summary>
        ///     The RocketMod settings.
        /// </summary>
        RocketSettings Settings { get; }
    }
}