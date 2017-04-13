using System;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.IO;
using Rocket.API.Providers.Logging;

namespace Rocket.Core.Providers.Logging
{

    public class Log4NetLoggingProvider : IRocketLoggingProvider
    {
        public void Load(bool isReload)
        {
            string logConfiguration = "log4net.config.xml";
            try
            {
                if (!File.Exists(logConfiguration))
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
                    File.WriteAllText(logConfiguration, config);
                }

                XmlConfigurator.Configure(new FileInfo(logConfiguration));

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

        public bool IsDebugEnabled => GetLogger().IsDebugEnabled;
        public bool IsInfoEnabled => GetLogger().IsInfoEnabled;
        public bool IsWarnEnabled => GetLogger().IsWarnEnabled;
        public bool IsErrorEnabled => GetLogger().IsErrorEnabled;
        public bool IsFatalEnabled => GetLogger().IsFatalEnabled;

        public void Debug(object message, Exception exception = null)
        {
            GetLogger().Debug(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.DEBUG, message));
        }

        public void Info(object message, Exception exception = null)
        {
            GetLogger().Info(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.INFO, message));
        }

        public void Warn(object message, Exception exception = null)
        {
            GetLogger().Warn(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.WARN, message));
        }

        public void Error(object message, Exception exception = null)
        {
            GetLogger().Error(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.ERROR, message));
        }
        
        public void Fatal(object message, Exception exception = null)
        {
            GetLogger().Fatal(message, exception);
            OnLog?.Invoke(new LogMessage(LogLevel.FATAL, message));
        }

        public void Unload()
        {

        }

        public event Action<LogMessage> OnLog;
    }
}
