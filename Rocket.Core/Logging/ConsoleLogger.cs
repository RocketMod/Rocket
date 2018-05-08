using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Configuration;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(IDependencyContainer container) : base(container) { }

        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null, ConsoleColor? color = null,
                                  params object[] bindings)
        {
            ConsoleColor orgCol = Console.ForegroundColor;

            SetColor(ConsoleColor.White);
            Console.Write("[");

            SetColor(GetLogLevelColor(level));
            Console.Write(GetLogLevelPrefix(level));

            SetColor(ConsoleColor.White);
            Console.Write("] ");

            if (RocketSettings?.Settings.IncludeMethodsInLogs ?? true)
            {
                SetColor(ConsoleColor.White);
                Console.Write("[");

                SetColor(ConsoleColor.DarkGray);
                Console.Write(GetLoggerCallingMethod().GetDebugName());

                SetColor(ConsoleColor.White);
                Console.Write("] ");
            }

            SetColor(color ?? ConsoleColor.White);
            Console.WriteLine(message);

            if (exception != null)
            {
                SetColor(ConsoleColor.Red);
                Console.WriteLine(exception);
            }

            SetColor(orgCol);
        }

        protected virtual void SetColor(ConsoleColor color)
        {
            if (Console.ForegroundColor != color)
                Console.ForegroundColor = color;
        }
    }
}