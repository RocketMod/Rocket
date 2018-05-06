using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    public abstract class BaseLogger : ILogger
    {
        protected BaseLogger()
        {
            SkipTypeFromLogging(GetType());
        }

        private static readonly ICollection<Type> ignoredLoggingTypes = new HashSet<Type> { typeof(BaseLogger), typeof(ProxyLogger), typeof(LoggingExtensions) };
        public static void SkipTypeFromLogging(Type type)
        {
            ignoredLoggingTypes.Add(type);
        }

        public void Log(string message, LogLevel level = LogLevel.Information, Exception exception = null, ConsoleColor? color = null, params object[] bindings)
        {
            if (!IsEnabled(level))
                return;

            OnLog(message, level, exception, color, bindings);
        }

        public abstract void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                          ConsoleColor? color = null, params object[] bindings);

        public string GetLogLevelPrefix(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "Trace";
                case LogLevel.Debug:
                    return "Debug";
                case LogLevel.Native:
                    return "Game";
                case LogLevel.Information:
                    return "Info";
                case LogLevel.Warning:
                    return "Warn";
                case LogLevel.Error:
                    return "Error";
                case LogLevel.Fatal:
                    return "Fatal";
            }
            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        public ConsoleColor GetLogLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return ConsoleColor.DarkGray;
                case LogLevel.Debug:
                    return ConsoleColor.Gray;
                case LogLevel.Native:
                    return ConsoleColor.White;
                case LogLevel.Information:
                    return ConsoleColor.Green;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                case LogLevel.Fatal:
                    return ConsoleColor.DarkRed;
            }
            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        public virtual MethodBase GetLoggerCallingMethod()
        {
            return ReflectionExtensions.GetCallingMethod(ignoredLoggingTypes.ToArray());
        }


        private readonly ICollection<LogLevel> enabledLevels = new HashSet<LogLevel>
        {
#if DEBUG
            //LogLevel.Trace, 
            LogLevel.Debug,
#endif
            LogLevel.Native,
            LogLevel.Information,
            LogLevel.Warning,
            LogLevel.Error,
            LogLevel.Fatal
        };

        public virtual bool IsEnabled(LogLevel level)
        {
            return enabledLevels.Contains(level);
        }

        public virtual void SetEnabled(LogLevel level, bool enabled)
        {
            if (enabled)
                enabledLevels.Add(level);
            else
                enabledLevels.Remove(level);
        }
    }
}