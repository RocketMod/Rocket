using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Rocket.Core.Logging
{
    public partial class Logger_
    {
        private static void ProcessInternalLog(ELogType type, string message, ConsoleColor color = ConsoleColor.White)
        {
            if (type == ELogType.Error || type == ELogType.Exception)
            {
                writeToConsole(message, ConsoleColor.Red);
            }
            else if (type == ELogType.Warning)
            {
                writeToConsole(message, ConsoleColor.Yellow);
            }
            else
            {
                writeToConsole(message, color);
            }
            ProcessLog(type, message);
        }

        private static void writeToConsole(string message, ConsoleColor color)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = old;
        }

        private static void ProcessLog(ELogType type, string message, bool rcon = true)
        {
            AsyncLoggerQueue.Current.Enqueue(new LogEntry() { Severity = type, Message = message, RCON = rcon });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ExternalLog(object message, ConsoleColor color)
        {
            ELogType severity;
            switch (color)
            {
                case ConsoleColor.Red:
                    severity = ELogType.Error;
                    break;

                case ConsoleColor.Yellow:
                    severity = ELogType.Warning;
                    break;

                default:
                    severity = ELogType.Info;
                    break;
            }
            ProcessLog(severity, message.ToString());
        }
    }
}