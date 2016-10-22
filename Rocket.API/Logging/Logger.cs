using System;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace Rocket.API.Logging
{
    public class Logger
    {
        public static void Initialize(string logFile)
        {

            if (!File.Exists(logFile))
            {
                var config = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<log4net>
    <appender name=""RollingFile"" type=""log4net.Appender.RollingFileAppender"">
    <file value=""Logs/Rocket.log"" />
    <appendToFile value=""true"" />
    <maximumFileSize value=""100KB"" />
    <maxSizeRollBackups value=""2"" />

    <layout type=""log4net.Layout.PatternLayout"">
        <conversionPattern value=""%level %thread %logger - %message%newline"" />
    </layout>
    </appender>
    <root>
    <level value=""DEBUG"" />
    <appender-ref ref=""RollingFile"" />
    </root>
</log4net>";
                File.WriteAllText(logFile, config);
            }
                ICollection configMessages = XmlConfigurator.Configure(new FileInfo(logFile));

                foreach (var msg in configMessages)
                {
                    System.IO.File.AppendAllText("test.txt", msg.ToString() + Environment.NewLine);
                }
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
