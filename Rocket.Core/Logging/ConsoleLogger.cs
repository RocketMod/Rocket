using System;
using System.Reflection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly string debugPrefix = "DEBUG";
        private readonly string errorPrefix = "ERROR";
        private readonly string fatalPrefix = "FATAL";
        private readonly string infoPrefix = "INFO";
        private readonly string tracePrefix = "TRACE";
        private readonly string warnPrefix = "WARN";

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

        public bool IsNativeEnabled => false;

        public void LogDebug(string message, params object[] arguments)
        {
            LogDebug(message, (ConsoleColor?) null, arguments);
        }

        public void LogDebug(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Cyan);
            Console.WriteLine(FormatMessage(message, debugPrefix, arguments));
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
            Console.WriteLine(FormatMessage(message, debugPrefix, arguments));
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogInformation(string message, params object[] arguments)
        {
            LogInformation(message, (ConsoleColor?) null, arguments);
        }

        public void LogError(string message, params object[] arguments)
        {
            LogError(message, (ConsoleColor?) null, arguments);
        }

        public void LogError(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine(FormatMessage(message, errorPrefix, arguments));
            SetColor(orgCol);
        }

        public void LogError(string message, Exception exception, params object[] arguments)
        {
            LogError(message, (ConsoleColor?) null, arguments);
        }

        public void LogError(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine(FormatMessage(message, errorPrefix, arguments));
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogFatal(string message, params object[] arguments)
        {
            LogFatal(message, (ConsoleColor?) null, arguments);
        }

        public void LogFatal(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine(FormatMessage(message, fatalPrefix, arguments));
            SetColor(orgCol);
        }

        public void LogFatal(string message, Exception exception, params object[] arguments)
        {
            LogFatal(message, (ConsoleColor?) null, arguments);
        }

        public void LogFatal(string message, Exception exception, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Red);
            Console.WriteLine(FormatMessage(message, fatalPrefix, arguments));
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogNative(string message, ConsoleColor? color, params object[] bindings)
        {
            //dont handle native log messages
        }

        public void LogInformation(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.White);
            Console.WriteLine(FormatMessage(message, infoPrefix, arguments));
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
            Console.WriteLine(FormatMessage(message, infoPrefix, arguments));
            SetColor(ConsoleColor.Red);
            Console.WriteLine(infoPrefix);
            SetColor(orgCol);
        }

        public void LogWarning(string message, params object[] arguments)
        {
            LogWarning(message, (ConsoleColor?) null, arguments);
        }

        public void LogTrace(string message, params object[] arguments)
        {
            LogTrace(message, (ConsoleColor?) null, arguments);
        }

        public void LogTrace(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Cyan);
            Console.WriteLine(FormatMessage(message, tracePrefix, arguments));
            SetColor(orgCol);
        }

        public void LogTrace(string message, Exception exception, params object[] arguments)
        {
            LogDebug(message, exception, null, arguments);
        }

        public void LogTrace(string message, Exception exception, ConsoleColor? color, params object[] bindings)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Cyan);
            Console.WriteLine(FormatMessage(message, tracePrefix, bindings));
            SetColor(ConsoleColor.Red);
            Console.WriteLine(exception);
            SetColor(orgCol);
        }

        public void LogWarning(string message, ConsoleColor? color, params object[] arguments)
        {
            ConsoleColor orgCol = Console.ForegroundColor;
            SetColor(color ?? ConsoleColor.Yellow);
            Console.WriteLine(FormatMessage(message, warnPrefix, arguments));
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
            Console.WriteLine(FormatMessage(message, warnPrefix, arguments));
            SetColor(ConsoleColor.Red);
            Console.WriteLine(warnPrefix);
            SetColor(orgCol);
        }

        private string FormatMessage(string message, string prefix, params object[] args)
        {
            MethodBase callingMethod =
                ReflectionExtensions.GetCallingMethod(typeof(ConsoleLogger), typeof(ProxyLogger));
            string format =
                $"[{prefix}] [{callingMethod.GetDebugName()}] {message}";
            return string.Format(format, args);
        }

        private void SetColor(ConsoleColor color)
        {
            if (Console.ForegroundColor != color) Console.ForegroundColor = color;
        }
    }
}