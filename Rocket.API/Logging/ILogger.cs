using System;
using Rocket.API.ServiceProxies;

namespace Rocket.API.Logging
{
    public interface ILogger : IProxyableService
    {
        bool IsTraceEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }

        void LogTrace(string message, params object[] arguments);
        void LogTrace(string message, ConsoleColor? color, params object[] arguments);

        void LogTrace(string message, Exception exception, params object[] arguments);
        void LogTrace(string message, Exception exception, ConsoleColor? color, params object[] arguments);

        void LogDebug(string message, params object[] arguments);
        void LogDebug(string message, ConsoleColor? color, params object[] arguments);

        void LogDebug(string message, Exception exception, params object[] arguments);
        void LogDebug(string message, Exception exception, ConsoleColor? color, params object[] arguments);

        void LogInformation(string message, params object[] arguments);
        void LogInformation(string message, ConsoleColor? color, params object[] arguments);

        void LogInformation(string message, Exception exception, params object[] arguments);
        void LogInformation(string message, Exception exception, ConsoleColor? color, params object[] arguments);

        void LogWarning(string message, params object[] arguments);
        void LogWarning(string message, ConsoleColor? color, params object[] arguments);

        void LogWarning(string message, Exception exception, params object[] arguments);
        void LogWarning(string message, Exception exception, ConsoleColor? color, params object[] arguments);

        void LogError(string message, params object[] arguments);
        void LogError(string message, ConsoleColor? color, params object[] arguments);

        void LogError(string message, Exception exception, params object[] arguments);
        void LogError(string message, Exception exception, ConsoleColor? color = null, params object[] arguments);

        void LogFatal(string message, params object[] arguments);
        void LogFatal(string message, ConsoleColor? color, params object[] arguments);

        void LogFatal(string message, Exception exception, params object[] arguments);
        void LogFatal(string message, Exception exception, ConsoleColor? color, params object[] arguments);
    }
}