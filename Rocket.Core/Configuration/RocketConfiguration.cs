namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     See <see cref="IRocketConfigurationProvider" />.
    /// </summary>
    public class RocketConfiguration
    {
        public LogSettings Logging { get; set; } = new LogSettings();
    }

    /// <summary>
    ///     Logging related settings.
    /// </summary>
    public class LogSettings
    {
        /// <summary>
        ///     The log level to show.
        /// </summary>
        public string LogLevel { get; set; } = API.Logging.LogLevel.Information.ToString();

        /// <summary>
        ///     Defines if the plugin loggers should be enabled by default.
        /// </summary>
        public bool EnableSeparatePluginLogs { get; set; } = true;

        /// <summary>
        ///     Defines if command executions should be logged.
        /// </summary>
        public bool EnableCommandExecutionsLogs { get; set; } = true;

        /// <summary>
        ///     Defines if methods should be included in the logs.
        /// </summary>
        public bool IncludeMethods { get; set; } = true;

        /// <summary>
        ///     Defines if the game output should be ignored.
        /// </summary>
        public bool IgnoreGameLogs { get; set; } = true;

        /// <summary>
        ///     Determines console mode.
        ///     Supported: "ANSI", "RGB", "DEFAULT", "COMPAT"
        /// </summary>
        public string ConsoleMode { get; set; } = "Default";
    }
}