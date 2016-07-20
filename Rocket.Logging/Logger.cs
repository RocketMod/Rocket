using System;
using log4net;
using log4net.Config;
using System.Diagnostics;

namespace Rocket.Logging
{
    public class Logger
    {
        public static void Initialize()
        {
            BasicConfigurator.Configure();
        }

        private static ILog GetLogger()
        {
            StackFrame frame = new StackFrame(2);
            var method = frame.GetMethod();
            return LogManager.GetLogger(method.DeclaringType);
        }


        public static bool IsDebugEnabled { get { return GetLogger().IsDebugEnabled; } }
        public static bool IsErrorEnabled { get { return GetLogger().IsErrorEnabled; } }
        public static bool IsFatalEnabled { get { return GetLogger().IsFatalEnabled; } }
        public static bool IsInfoEnabled { get { return GetLogger().IsInfoEnabled; } }
        public static bool IsWarnEnabled { get { return GetLogger().IsWarnEnabled; } }

        public static void Debug(object message)
        {
            GetLogger().Debug(message);
        }
        public static void Debug(object message, Exception exception)
        {
            GetLogger().Debug(message, exception);
        }
        public static void Error(object message)
        {
            GetLogger().Error(message);
        }
        public static void Error(object message, Exception exception)
        {
            GetLogger().Error(message, exception);
        }
        public static void Fatal(object message)
        {
            GetLogger().Fatal(message);
        }
        public static void Fatal(object message, Exception exception)
        {
            GetLogger().Fatal(message, exception);
        }
        public static void Info(object message)
        {
            GetLogger().Info(message);
        }
        public static void Info(object message, Exception exception)
        {
            GetLogger().Info(message, exception);
        }
        public static void Warn(object message)
        {
            GetLogger().Warn(message);
        }
        public static void Warn(object message, Exception exception)
        {
            GetLogger().Warn(message, exception);
        }
    }
}
