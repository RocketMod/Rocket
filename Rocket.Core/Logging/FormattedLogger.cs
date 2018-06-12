using System;
using System.Drawing;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    public abstract class FormattedLogger : BaseLogger
    {
        protected static readonly object ConsoleLock = new object();

        protected FormattedLogger(IDependencyContainer container) : base(container) { }

        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                                   params object[] bindings)
        {
            OnLog(message, level, exception, null, bindings);
        }

        public void WriteLine(LogLevel level, string message, Color? color = null, params object[] bindings)
        {
            lock (ConsoleLock)
            {
                WriteColored("[", Color.White);
                WriteColored(GetLogLevelPrefix(level), GetLogLevelColor(level));
                WriteColored("] ", Color.White);

                if (LogSettings.IncludeMethods)
                {
                    WriteColored("[", Color.White);
                    WriteColored(GetLoggerCallingMethod().GetDebugName(), Color.DarkGray);
                    WriteColored("] ", Color.White);
                }

                WriteLineColored(message, color, bindings);
                Console.ResetColor();
            }
        }

        public void Write(string format, Color? color = null, params object[] bindings)
        {
            WriteColored(format, color, bindings);
        }

        protected abstract void WriteColored(string format, Color? color = null, params object[] bindings);

        protected abstract void WriteLineColored(string format, Color? color = null, params object[] bindings);
    }
}