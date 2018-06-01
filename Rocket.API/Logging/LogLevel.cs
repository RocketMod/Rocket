namespace Rocket.API.Logging
{
    /// <summary>
    ///     Log levels for <see cref="ILogger" />.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     Trace messages.
        /// </summary>
        Trace,

        /// <summary>
        ///     Debug messages.
        /// </summary>
        Debug,

        /// <summary>
        ///     Game game messages.
        /// </summary>
        Game,

        /// <summary>
        ///     Information messages.
        /// </summary>
        Information,

        /// <summary>
        ///     Warning messages.
        /// </summary>
        Warning,

        /// <summary>
        ///     Error messages.
        /// </summary>
        Error,

        /// <summary>
        ///     Fatal messages which require immediate action.
        /// </summary>
        Fatal
    }
}
