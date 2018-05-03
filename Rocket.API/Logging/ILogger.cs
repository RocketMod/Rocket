using System;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Logging
{
    /// <summary>
    ///     A provider for logging.
    /// </summary>
    public interface ILogger : IProxyableService
    {
        /// <summary>
        ///     Checks if trace messages are enabled.
        /// </summary>
        bool IsTraceEnabled { get; }

        /// <summary>
        ///     Checks if debug messages are enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        ///     Checks if info messages are enabled.
        /// </summary>
        bool IsInformationEnabled { get; }

        /// <summary>
        ///     Checks if warn messages are enabled.
        /// </summary>
        bool IsWarningEnabled { get; }

        /// <summary>
        ///     Checks if error messages are enabled.
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        ///     Checks if fatal error messages are enabled.
        /// </summary>
        bool IsFatalEnabled { get; }

        /// <summary>
        ///     Checks if native game messages are enabled.
        /// </summary>
        bool IsNativeEnabled { get; }

        /// <summary>
        ///     Logs a trace message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogTrace(string message, params object[] bindings);

        /// <summary>
        ///     Logs a trace message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogTrace(string message, ConsoleColor? color, params object[] bindings);

        /// <summary>
        ///     Logs a trace message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogTrace(string message, Exception exception, params object[] bindings);

        /// <summary>
        ///     Logs a trace message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogTrace(string message, Exception exception, ConsoleColor? color, params object[] bindings);


        /// <summary>
        ///     Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogDebug(string message, params object[] bindings);
        /// <summary>
        ///     Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogDebug(string message, ConsoleColor? color, params object[] bindings);

        /// <summary>
        ///     Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogDebug(string message, Exception exception, params object[] bindings);

        /// <summary>
        ///     Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogDebug(string message, Exception exception, ConsoleColor? color, params object[] bindings);


        /// <summary>
        ///     Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogInformation(string message, params object[] bindings);

        /// <summary>
        ///     Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogInformation(string message, ConsoleColor? color, params object[] bindings);

        /// <summary>
        ///     Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogInformation(string message, Exception exception, params object[] bindings);

        /// <summary>
        ///     Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogInformation(string message, Exception exception, ConsoleColor? color, params object[] bindings);


        /// <summary>
        ///     Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogWarning(string message, params object[] bindings);

        /// <summary>
        ///     Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogWarning(string message, ConsoleColor? color, params object[] bindings);

        /// <summary>
        ///     Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogWarning(string message, Exception exception, params object[] bindings);

        /// <summary>
        ///     Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogWarning(string message, Exception exception, ConsoleColor? color, params object[] bindings);


        /// <summary>
        ///     Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogError(string message, params object[] bindings);

        /// <summary>
        ///     Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogError(string message, ConsoleColor? color, params object[] bindings);

        /// <summary>
        ///     Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogError(string message, Exception exception, params object[] bindings);

        /// <summary>
        ///     Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogError(string message, Exception exception, ConsoleColor? color = null, params object[] bindings);

        /// <summary>
        ///     Logs a fatal error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogFatal(string message, params object[] bindings);

        /// <summary>
        ///     Logs a fatal error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogFatal(string message, ConsoleColor? color, params object[] bindings);

        /// <summary>
        ///     Logs a fatal error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogFatal(string message, Exception exception, params object[] bindings);

        /// <summary>
        ///     Logs a fatal error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The associated exception.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogFatal(string message, Exception exception, ConsoleColor? color, params object[] bindings);

        /// <summary>
        ///     Logs a native (game) message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="bindings">The bindings of the message. See <see cref="string.Format(string, object[])"/>.</param>
        void LogNative(string message, ConsoleColor? color, params object[] bindings);
    }
}