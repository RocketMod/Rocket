using System;
using Rocket.API.Logging;

namespace Rocket.Core.Logging
{
    public static class LoggingExtensions
    {
        public static void LogTrace(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogTrace(message, null, null, arguments);
        }

        public static void LogTrace(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.LogTrace(message, exception, null, arguments);
        }
        
        public static void LogDebug(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogDebug(message, null, null, arguments);
        }

        public static void LogDebug(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.LogDebug(message, exception, null, arguments);
        }

        public static void LogDebug(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(message, LogLevel.Debug, exception, arguments);
        }

        public static void LogNative(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogNative(message, null, null, arguments);
        }

        public static void LogNative(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogNative(message, null, arguments);
        }

        public static void LogNative(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.LogNative(message, exception, null, arguments);
        }

        public static void LogNative(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(message, LogLevel.Native, exception, arguments);
        }

        public static void LogInformation(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogInformation(message, null, null, arguments);
        }

        public static void LogInformation(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogInformation(message, null, arguments);
        }

        public static void LogInformation(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.LogInformation(message, exception, null, arguments);
        }

        public static void LogInformation(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(message, LogLevel.Information, exception, arguments);
        }

        public static void LogWarning(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogWarning(message, null, null, arguments);
        }

        public static void LogWarning(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogWarning(message, null, arguments);
        }

        public static void LogWarning(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.LogWarning(message, exception, null, arguments);
        }

        public static void LogWarning(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(message, LogLevel.Warning, exception, arguments);
        }

        public static void LogError(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogError(message, null, null, arguments);
        }

        public static void LogError(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogError(message, null, arguments);
        }

        public static void LogError(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.LogError(message, exception, null, arguments);
        }

        public static void LogError(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.Log(message, LogLevel.Error, exception, arguments);
        }

        public static void LogFatal(this ILogger logger, string message, params object[] arguments)
        {
            logger.LogFatal(message, null, null, arguments);
        }

        public static void LogFatal(this ILogger logger, string message, Exception exception, params object[] arguments)
        {
            logger.LogFatal(message, exception, null, arguments);
        }
    }
}