using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        public virtual void Log(string message, LogLevel level = LogLevel.Information, Exception exception = null, ConsoleColor? color = null,
                        params object[] bindings)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            
            SetColor(ConsoleColor.White);
            Console.Write("[");

            SetColor(GetLogLevelColor(level));
            Console.Write(GetLogLevelPrefix(level));

            SetColor(ConsoleColor.White);
            Console.Write("] ");

            MethodBase callingMethod =
                ReflectionExtensions.GetCallingMethod(typeof(ConsoleLogger), typeof(ProxyLogger), typeof(LoggingExtensions));

            SetColor(ConsoleColor.White);
            Console.Write("[");

            SetColor(ConsoleColor.DarkGray);
            Console.Write(callingMethod.GetDebugName());

            SetColor(ConsoleColor.White);
            Console.Write("] ");

            SetColor(color ?? ConsoleColor.White);
            Console.WriteLine(message);

            if (exception != null)
            {
                SetColor(ConsoleColor.Red);
                Console.WriteLine(exception);
            }

            SetColor(orgCol);
        }

        public static string GetLogLevelPrefix(LogLevel level)
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

        public static ConsoleColor GetLogLevelColor(LogLevel level)
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

        protected virtual void SetColor(ConsoleColor color)
        {
            if (Console.ForegroundColor != color)
                Console.ForegroundColor = color;
        }

        private readonly ICollection<LogLevel> enabledLevels = new HashSet<LogLevel>
        {
#if DEBUG
            LogLevel.Trace, 
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