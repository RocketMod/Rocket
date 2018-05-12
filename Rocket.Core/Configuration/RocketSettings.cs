namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     See <see cref="IRocketSettingsProvider" />.
    /// </summary>
    public class RocketSettings
    {
        public Logging Logging { get; set; } = new Logging();
    }

    public class Logging
    {
        /// <summary>
        ///     Defines if the plugin loggers should be enabled by default.
        /// </summary>
        public bool PluginLogsEnabled { get; set; } = true;

        /// <summary>
        ///     Defines if command executions should be logged.
        /// </summary>
        public bool LogCommandExecutions { get; set; } = true;

        /// <summary>
        ///     Defines if methods should be included in the logs.
        /// </summary>
        public bool IncludeMethodsInLogs { get; set; } = true;
    }
}