using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Core
{
    public class ConsoleLogger : ILog
    {
        private readonly string tracePrefix = "[TRACE]";
        private readonly string debugPrefix = "[DEBUG]";
        private readonly string infoPrefix = "[INFO]";
        private readonly string warnPrefix = "[WARN]";
        private readonly string errorPrefix = "[ERROR]";
        private readonly string fatalPrefix = "[FATAL]";

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

        private void setColor(ConsoleColor color)
        {
            if (Console.ForegroundColor != color) Console.ForegroundColor = color;
        }

        public void Debug(string message, params object[] arguments)
        {
            setColor(ConsoleColor.Cyan);
            Console.Write($"{debugPrefix} {message}", arguments);
        }

        public void Debug(string message, Exception exception, params object[] arguments)
        {
            setColor(ConsoleColor.Cyan);
            Console.Write($"{debugPrefix} {message}", arguments);
            setColor(ConsoleColor.Red);
            Console.WriteLine(exception);
        }

        public void Error(string message, params object[] arguments)
        {
            setColor(ConsoleColor.Red);
            Console.Write($"{errorPrefix} {message}", arguments);
        }

        public void Error(string message, Exception exception, params object[] arguments)
        {
            setColor(ConsoleColor.Red);
            Console.Write($"{errorPrefix} {message}", arguments);
            setColor(ConsoleColor.Red);
            Console.WriteLine(exception);
        }

        public void Fatal(string message, params object[] arguments)
        {
            setColor(ConsoleColor.Red);
            Console.Write($"{fatalPrefix} {message}", arguments);
        }

        public void Fatal(string message, Exception exception, params object[] arguments)
        {
            setColor(ConsoleColor.Red);
            Console.Write($"{fatalPrefix} {message}", arguments);
            setColor(ConsoleColor.Red);
            Console.WriteLine(exception);
        }

        public void Info(string message, params object[] arguments)
        {
            setColor(ConsoleColor.White);
            Console.Write($"{infoPrefix} {message}", arguments);
        }

        public void Info(string message, Exception exception, params object[] arguments)
        {
            setColor(ConsoleColor.White);
            Console.Write($"{fatalPrefix} {message}", arguments);
            setColor(ConsoleColor.Red);
            Console.WriteLine(infoPrefix);
        }

        public void Trace(string message, params object[] arguments)
        {
            setColor(ConsoleColor.Cyan);
            Console.Write($"{tracePrefix} {message}", arguments);
        }

        public void Trace(string message, Exception exception, params object[] arguments)
        {
            setColor(ConsoleColor.Cyan);
            Console.Write($"{tracePrefix} {message}", arguments);
            setColor(ConsoleColor.Red);
            Console.WriteLine(exception);
        }

        public void Warning(string message, params object[] arguments)
        {
            setColor(ConsoleColor.Yellow);
            Console.Write($"{warnPrefix} {message}", arguments);
        }

        public void Warning(string message, Exception exception, params object[] arguments)
        {
            setColor(ConsoleColor.Yellow);
            Console.Write($"{tracePrefix} {message}", arguments);
            setColor(ConsoleColor.Red);
            Console.WriteLine(warnPrefix);
        }
    }
}
