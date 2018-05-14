using Rocket.API.Logging;

namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     See <see cref="IRocketSettingsProvider" />.
    /// </summary>
    public class RocketSettings
    {
        public LogSettings Logging { get; set; } = new LogSettings();
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();
    }

    /// <summary>
    ///     Provides common database settings for plugins.
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        ///     The database name.
        /// </summary>
        public string DatabaseName { get; set; } = "unturned";

        /// <summary>
        ///     The database host.
        /// </summary>
        public string DatabaseHost { get; set; } = "localhost";

        /// <summary>
        ///     The database port.
        /// </summary>
        public ushort DatabasePort { get; set; } = 3306;

        /// <summary>
        ///     The username for database.
        /// </summary>
        public string DatabaseUsername { get; set; } = "root";

        /// <summary>
        ///     The password for the database.
        /// </summary>
        public string DatabasePassword { get; set; } = "hi";

        /// <summary>
        ///     The database provider type.
        /// </summary>
        public string DatabaseProvider { get; set; } = "MySQL";
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
    }
}