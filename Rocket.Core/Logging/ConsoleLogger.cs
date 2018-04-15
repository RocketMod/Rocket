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
            throw new NotImplementedException();
        }

        public void LogDebug(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Cyan);
            Console.WriteLine($"{debugPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogDebug(string message, Exception exception, params object[] arguments)
        {
            LogDebug(message, exception, null, arguments);
        }

        public void LogDebug(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Cyan);
            Console.WriteLine($"{debugPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogInformation(string message, params object[] arguments)
        {
            LogInformation(message, (ConsoleColor?)null, arguments);
        }

        public void LogError(string message, params object[] arguments)
        {
            LogError(message, (ConsoleColor?)null, arguments);
        }

        public void LogError(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine($"{errorPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogError(string message, Exception exception, params object[] arguments)
        {
            LogError(message, (ConsoleColor?)null, arguments);
        }

        public void LogError(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine($"{errorPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogFatal(string message, params object[] arguments)
        {
            LogFatal(message, (ConsoleColor?)null, arguments);
        }

        public void LogFatal(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine($"{fatalPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogFatal(string message, Exception exception, params object[] arguments)
        {
            LogFatal(message, (ConsoleColor?)null, arguments);
        }

        public void LogFatal(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine($"{fatalPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogInformation(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.White);
            Console.WriteLine($"{infoPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogInformation(string message, Exception exception, params object[] arguments)
        {
            LogInformation(message, exception, null, arguments);
        }

        public void LogInformation(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.White);
            Console.WriteLine($"{fatalPrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(infoPrefix);
            SetColor(orgCol);
        }

        public void LogWarning(string message, params object[] arguments)
        {
            LogWarning(message, (ConsoleColor?)null, arguments);
        }

        public void LogTrace(string message, params object[] arguments)
        {
            LogTrace(message, (ConsoleColor?)null, arguments);
        }

        public void LogTrace(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Cyan);
            Console.WriteLine($"{tracePrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogTrace(string message, Exception exception, params object[] arguments)
        {
            LogDebug(message, exception, null, arguments);
        }

        public void LogTrace(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Cyan);
            Console.WriteLine($"{tracePrefix} {message}", arguments);
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogWarning(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Yellow);
            Console.WriteLine($"{warnPrefix} {message}", arguments);
            SetColor(orgCol);
        }

        public void LogWarning(string message, Exception exception, params object[] arguments)
        {
            LogWarning(message, exception, null, arguments);
        }

        public void LogWarning(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Yellow);
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