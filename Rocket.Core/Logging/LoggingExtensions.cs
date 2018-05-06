using System;
using Rocket.API.Logging;

namespace Rocket.Core.Logging
{
    public static class LoggingExtensions
    {
        public static void LogTrace(this ILogger logger, string message, params object[] bindings)
        {
            logger.LogTrace(message, null, null, bindings);
        }

        public static void LogTrace(this ILogger logger, string message, ConsoleColor? color, params object[] bindings)
        {
            logger.LogTrace(message, null, color, bindings);
        }

        public static void LogTrace(this ILogger logger, string message, Exception exception, params object[] bindings)
        {
            logger.LogTrace(message, exception, null, bindings);
        }

        public static void LogTrace(this ILogger logger, string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            logger.Log(message, LogLevel.Trace, exception, color, bindings);
        }

        public static void LogDebug(this ILogger logger, string message, params object[] bindings)
        {
            logger.LogDebug(message, null, null, bindings);
        }

        public static void LogDebug(this ILogger logger, string message, ConsoleColor? color, params object[] bindings)
        {
            logger.LogDebug(message, null, color, bindings);
        }

        public static void LogDebug(this ILogger logger, string message, Exception exception, params object[] bindings)
        {
            logger.LogDebug(message, exception, null, bindings);
        }

        public static void LogDebug(this ILogger logger, string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            logger.Log(message, LogLevel.Debug, exception, color, bindings);
        }

        public static void LogNative(this ILogger logger, string message, params object[] bindings)
        {
            logger.LogNative(message, null, null, bindings);
        }

        public static void LogNative(this ILogger logger, string message, ConsoleColor? color, params object[] bindings)
        {
            logger.LogNative(message, color, null, bindings);
        }

        public static void LogNative(this ILogger logger, string message, Exception exception, params object[] bindings)
        {
            logger.LogNative(message, exception, null, bindings);
        }

        public static void LogNative(this ILogger logger, string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            logger.Log(message, LogLevel.Native, exception, color, bindings);
        }

        public static void LogInformation(this ILogger logger, string message, params object[] bindings)
        {
            logger.LogInformation(message, null, null, bindings);
        }

        public static void LogInformation(this ILogger logger, string message, ConsoleColor? color, params object[] bindings)
        {
            logger.LogInformation(message, null, color, bindings);
        }

        public static void LogInformation(this ILogger logger, string message, Exception exception, params object[] bindings)
        {
            logger.LogInformation(message, exception, null, bindings);
        }

        public static void LogInformation(this ILogger logger, string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            logger.Log(message, LogLevel.Information, exception, color, bindings);
        }

        public static void LogWarning(this ILogger logger, string message, params object[] bindings)
        {
            logger.LogWarning(message, null, null, bindings);
        }

        public static void LogWarning(this ILogger logger, string message, ConsoleColor? color, params object[] bindings)
        {
            logger.LogWarning(message, null, color, bindings);
        }

        public static void LogWarning(this ILogger logger, string message, Exception exception, params object[] bindings)
        {
            logger.LogWarning(message, exception, null, bindings);
        }

        public static void LogWarning(this ILogger logger, string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            logger.Log(message, LogLevel.Warning, exception, color, bindings);
        }

        public static void LogError(this ILogger logger, string message, params object[] bindings)
        {
            logger.LogError(message, null, null, bindings);
        }

        public static void LogError(this ILogger logger, string message, ConsoleColor? color, params object[] bindings)
        {
            logger.LogError(message, null, color, bindings);
        }

        public static void LogError(this ILogger logger, string message, Exception exception, params object[] bindings)
        {
            logger.LogError(message, exception, null, bindings);
        }

        public static void LogError(this ILogger logger, string message, Exception exception, ConsoleColor? color = null, params object[] bindings)
        {
            logger.Log(message, LogLevel.Error, exception, color, bindings);
        }

        public static void LogFatal(this ILogger logger, string message, params object[] bindings)
        {
            logger.LogFatal(message, null, null, bindings);
        }

        public static void LogFatal(this ILogger logger, string message, ConsoleColor? color, params object[] bindings)
        {
            logger.LogFatal(message, color, null, bindings);
        }

        public static void LogFatal(this ILogger logger, string message, Exception exception, params object[] bindings)
        {
            logger.LogFatal(message, exception, null, bindings);
        }

        public static void LogFatal(this ILogger logger, string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            logger.Log(message, LogLevel.Fatal, exception, color, bindings);
        }
    }
}