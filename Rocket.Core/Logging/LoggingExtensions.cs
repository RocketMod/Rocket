using System;
using Rocket.API.Logging;

namespace Rocket.Core.Logging
{
    public static class LoggingExtensions
    {
        public static void LogTrace(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogTrace(message, null, arguments);
        }

        public static void LogTrace(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(LogLevel.Trace, message, exception, arguments);
        }

        public static void LogDebug(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogDebug(message, null, arguments);
        }

        public static void LogDebug(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(LogLevel.Debug, message, exception, arguments);
        }

        public static void LogNative(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogNative(message, null, arguments);
        }

        public static void LogNative(this ILogger logger, string message, Exception exception,
                                     params object[] arguments)
        {
            logger.Log(LogLevel.Game, message, exception, arguments);
        }

        public static void LogInformation(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogInformation(message, null, arguments);
        }

        public static void LogInformation(this ILogger logger, string message, Exception exception,
                                          params object[] arguments)
        {
            logger.Log(LogLevel.Information, message, exception, arguments);
        }

        public static void LogWarning(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogWarning(message, null, arguments);
        }

        public static void LogWarning(this ILogger logger, string message, Exception exception,
                                      params object[] arguments)
        {
            logger.Log(LogLevel.Warning, message, exception, arguments);
        }

        public static void LogError(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogError(message, null, arguments);
        }

        public static void LogError(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(LogLevel.Error, message, exception, arguments);
        }

        public static void LogFatal(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogFatal(message, null, arguments);
        }

        public static void LogFatal(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(LogLevel.Fatal, message, exception, arguments);
        }
    }
}