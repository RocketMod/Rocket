using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Core
{
    public interface ILog
    {
        bool IsTraceEnabled { get; }
        void Trace(string message, params object[] arguments);
        void Trace(string message, Exception exception, params object[] arguments);
        bool IsDebugEnabled { get; }
        void Debug(string message, params object[] arguments);
        void Debug(string message, Exception exception, params object[] arguments);
        bool IsInfoEnabled { get; }
        void Info(string message, params object[] arguments);
        void Info(string message, Exception exception, params object[] arguments);
        bool IsWarnEnabled { get; }
        void Warning(string message, params object[] arguments);
        void Warning(string message, Exception exception, params object[] arguments);
        bool IsErrorEnabled { get; }
        void Error(string message, params object[] arguments);
        void Error(string message, Exception exception, params object[] arguments);
        bool IsFatalEnabled { get; }
        void Fatal(string message, params object[] arguments);
        void Fatal(string message, Exception exception, params object[] arguments);
    }
}
