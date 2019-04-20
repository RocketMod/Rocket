using System;
using System.Drawing;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    public abstract class FormattedLogger : BaseLogger
    {
        public readonly object OutputLock = new object();

        protected FormattedLogger(IDependencyContainer container) : base(container)
        {
            SkipTypeFromLogging(GetType());
        }

        public override void OnLog(object message, LogLevel level = LogLevel.Information, Exception exception = null,
                                   params object[] bindings)
        {
            if (message != null)
                WriteLine(level, message, Color.White, bindings);

            if (exception != null)
                WriteLine(level, exception.ToString(), Color.Red);
        }

        public void WriteLine(LogLevel level, object message, Color? color = null, params object[] bindings)
        {
            lock (OutputLock)
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

                WriteLineColored(message.ToString(), color, bindings);
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