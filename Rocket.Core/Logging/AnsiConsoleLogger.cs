using System;
using System.Drawing;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Logging
{
    public class AnsiConsoleLogger : ConsoleLogger
    {
        private const string ESC = "\u001b[";

        public AnsiConsoleLogger(IDependencyContainer container) : base(container)
        {
        }

        protected override void WriteColored(string format, Color? color = null, params object[] bindings)
        {
            if (color == null)
            {
                Console.Write(format, bindings);
                return;
            }

            Console.Write(ESC + $"38;2;{color.Value.R};{color.Value.G};{color.Value.B}m" + format, bindings);
            
            //reset color 
            Console.Write(ESC + "0m");
        }

        protected override void WriteLineColored(string format, Color? color = null, params object[] bindings)
        {
            if (color == null)
            {
                Console.WriteLine(format, bindings);
                return;
            }

            Console.WriteLine(ESC + $"38;2;{color.Value.R};{color.Value.G};{color.Value.B}m" + format, bindings);

            //reset color
            Console.Write(ESC + "0m");
        }

        public override string ServiceName => "AnsiConsole";
    }
}