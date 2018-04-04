using System;

namespace Rocket.API.Logging {
    public interface ILogger {
        bool IsTraceEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        void Trace(string message, params object[] arguments);
        void Trace(string message, Exception exception, params object[] arguments);
        void Debug(string message, params object[] arguments);
        void Debug(string message, Exception exception, params object[] arguments);
        void Info(string message, params object[] arguments);
        void Info(string message, Exception exception, params object[] arguments);
        void Warning(string message, params object[] arguments);
        void Warning(string message, Exception exception, params object[] arguments);
        void Error(string message, params object[] arguments);
        void Error(string message, Exception exception, params object[] arguments);
        void Fatal(string message, params object[] arguments);
        void Fatal(string message, Exception exception, params object[] arguments);
    }
}