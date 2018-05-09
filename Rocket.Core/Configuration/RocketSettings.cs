namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     See <see cref="IRocketSettingsProvider" />.
    /// </summary>
    public class RocketSettings
    {
        /// <summary>
        ///     Defines if the plugin loggers should be enabled by default.
        /// </summary>
        public bool PluginLogsEnabled { get; } = true;

        /// <summary>
        ///     Defines if command executions should be logged.
        /// </summary>
        public bool LogCommandExecutions { get; } = true;

        /// <summary>
        ///     Defines if methods should be included in the logs.
        /// </summary>
        public bool IncludeMethodsInLogs { get; } = true;
    }
}