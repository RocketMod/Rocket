using System;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Logging
{
    public class ProxyLogger : ServiceProxy<ILogger>, ILogger
    {
        public ProxyLogger(IDependencyContainer container) : base(container) { }

        public bool IsTraceEnabled => throw new NotSupportedException("Not supported on proxy");
        public bool IsDebugEnabled => throw new NotSupportedException("Not supported on proxy");
        public bool IsInfoEnabled => throw new NotSupportedException("Not supported on proxy");
        public bool IsWarnEnabled => throw new NotSupportedException("Not supported on proxy");
        public bool IsErrorEnabled => throw new NotSupportedException("Not supported on proxy");
        public bool IsFatalEnabled => throw new NotSupportedException("Not supported on proxy");

        public void LogTrace(string message, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogTrace(message, arguments);
        }

        public void LogTrace(string message, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogTrace(message, color, arguments);
        }

        public void LogTrace(string message, Exception exception, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogTrace(message, exception, arguments);
        }

        public void LogTrace(string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogTrace(message, exception, color, bindings);
        }

        public void LogDebug(string message, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogDebug(message, arguments);
        }

        public void LogDebug(string message, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogDebug(message, color, arguments);
        }

        public void LogDebug(string message, Exception exception, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogDebug(message, exception, arguments);
        }

        public void LogDebug(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogDebug(message, exception, color, arguments);
        }

        public void LogInformation(string message, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogInformation(message, arguments);
        }

        public void LogInformation(string message, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogInformation(message, color, arguments);
        }

        public void LogInformation(string message, Exception exception, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogInformation(message, exception, arguments);
        }

        public void LogInformation(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogInformation(message, exception, color, arguments);
        }

        public void LogWarning(string message, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogWarning(message, arguments);
        }

        public void LogWarning(string message, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogWarning(message, color, arguments);
        }

        public void LogWarning(string message, Exception exception, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogWarning(message, exception, arguments);
        }

        public void LogWarning(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogWarning(message, exception, color, arguments);
        }

        public void LogError(string message, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogError(message, arguments);
        }

        public void LogError(string message, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogError(message, color, arguments);
        }

        public void LogError(string message, Exception exception, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogError(message, exception, arguments);
        }

        public void LogError(string message, Exception exception, ConsoleColor? color = null, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogError(message, exception, color, arguments);
        }

        public void LogFatal(string message, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogFatal(message, arguments);
        }

        public void LogFatal(string message, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogFatal(message, color, arguments);
        }

        public void LogFatal(string message, Exception exception, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogFatal(message, exception, arguments);
        }

        public void LogFatal(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            foreach (ILogger logger in ProxiedServices)
                logger.LogFatal(message, exception, color, arguments);
        }
    }
}