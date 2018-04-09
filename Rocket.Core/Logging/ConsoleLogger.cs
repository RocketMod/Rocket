using System;
using Rocket.API.Logging;

namespace Rocket.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly string debugPrefix = "[DEBUG]";
        private readonly string errorPrefix = "[ERROR]";
        private readonly string fatalPrefix = "[FATAL]";
        private readonly string infoPrefix = "[INFO]";
        private readonly string tracePrefix = "[TRACE]";
        private readonly string warnPrefix = "[WARN]";

        public bool IsTraceEnabled
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public bool IsDebugEnabled
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public bool IsInfoEnabled => true;

        public bool IsWarnEnabled => true;

        public bool IsErrorEnabled => true;

        public bool IsFatalEnabled => true;

        public void LogDebug(string message, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Cyan);
            Console.WriteLine($"{debugPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogDebug(string message, Exception exception, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Cyan);
            Console.WriteLine($"{debugPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogError(string message, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Red);
            Console.WriteLine($"{errorPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogError(string message, Exception exception, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Red);
            Console.WriteLine($"{errorPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogFatal(string message, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Red);
            Console.WriteLine($"{fatalPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogFatal(string message, Exception exception, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Red);
            Console.WriteLine($"{fatalPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogInformation(string message, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.White);
            Console.WriteLine($"{infoPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogInformation(string message, Exception exception, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.White);
            Console.WriteLine($"{fatalPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(infoPrefix);
            SetColor(orgCol);
        }

        public void LogTrace(string message, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Cyan);
            Console.WriteLine($"{tracePrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogTrace(string message, Exception exception, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Cyan);
            Console.WriteLine($"{tracePrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogWarning(string message, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine($"{warnPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogWarning(string message, Exception exception, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine($"{tracePrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(warnPrefix);
            SetColor(orgCol);
        }

        private void SetColor(ConsoleColor color)
        {
            if (Console.ForegroundColor != color) Console.ForegroundColor = color;
        }
    }
}