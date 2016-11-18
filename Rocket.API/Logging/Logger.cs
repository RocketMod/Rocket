using System;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;

namespace Rocket.API.Logging
{
    public enum LogLevel { DEBUG, INFO, WARN, ERROR, FATAL };
    public class Logger
    {
        public delegate void Log(LogMessage message);
        public static event Log OnLog;

        public static void Initialize(string logFile)
        {
            try
            {
                if (!File.Exists(logFile))
                {
                    var config = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                        <log4net>
                            <appender name=""RollingFile"" type=""log4net.Appender.RollingFileAppender"">
                            <file value=""Logs/Rocket.log"" />
                            <appendToFile value=""false"" />
                            <maximumFileSize value=""5MB"" />
                            <maxSizeRollBackups value=""-1"" />
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

                XmlConfigurator.Configure(new FileInfo(logFile));

                AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
                {
                    Fatal(e.ExceptionObject);
                };
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.txt",ex.ToString());
            }

        }


        private static ILog GetLogger()
        {
            StackFrame frame = new StackFrame(2);
            var method = frame.GetMethod();
            return LogManager.GetLogger(method.DeclaringType);
        }

        public static bool IsDebugEnabled { get { return GetLogger().IsDebugEnabled; } }
        public static bool IsInfoEnabled { get { return GetLogger().IsInfoEnabled; } }
        public static bool IsWarnEnabled { get { return GetLogger().IsWarnEnabled; } }
        public static bool IsErrorEnabled { get { return GetLogger().IsErrorEnabled; } }
        public static bool IsFatalEnabled { get { return GetLogger().IsFatalEnabled; } }

        public static void Debug(object message, Exception exception = null)
        {
            GetLogger().Debug(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.DEBUG, message, exception));
        }

        public static void Info(object message, Exception exception = null)
        {
            GetLogger().Info(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.INFO, message, exception));
        }

        public static void Warn(object message, Exception exception = null)
        {
            GetLogger().Warn(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.WARN, message, exception));
        }

        public static void Error(object message, Exception exception = null)
        {
            GetLogger().Error(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.ERROR, message, exception));
        }
        
        public static void Fatal(object message, Exception exception = null)
        {
            GetLogger().Fatal(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.FATAL, message, exception));
        }
    }
}
