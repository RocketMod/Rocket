using System;
using System.Drawing;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(IDependencyContainer container) : base(container) { }

        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                                   params object[] bindings)
        {
            if (message != null)
                WriteLine(level, message, false, bindings);

            if (exception != null)
                WriteLine(level, exception.ToString(), true);
        }

        private void WriteLine(LogLevel level, string message, bool isException, params object[] bindings)
        {
            Color orgCol = GetForegroundColor();

            SetForegroundColor(Color.White);
            Console.Write("[");

            SetForegroundColor(GetLogLevelColor(level));
            Console.Write(GetLogLevelPrefix(level));

            SetForegroundColor(Color.White);
            Console.Write("] ");

            if (LogSettings.IncludeMethods)
            {
                SetForegroundColor(Color.White);
                Console.Write("[");

                SetForegroundColor(Color.DarkGray);
                Console.Write(GetLoggerCallingMethod().GetDebugName());

                SetForegroundColor(Color.White);
                Console.Write("] ");
            }

            SetForegroundColor(isException ? Color.Red : Color.White);
            Console.WriteLine(message, bindings);

            SetForegroundColor(orgCol);
        }

        public override string ServiceName => "Console";

        public static Color GetForegroundColor()
        {
            int[] cColors =
            {
                0x000000, //Black = 0
                0x000080, //DarkBlue = 1
                0x008000, //DarkGreen = 2
                0x008080, //DarkCyan = 3
                0x800000, //DarkRed = 4
                0x800080, //DarkMagenta = 5
                0x808000, //DarkYellow = 6
                0xC0C0C0, //Gray = 7
                0x808080, //DarkGray = 8
                0x0000FF, //Blue = 9
                0x00FF00, //Green = 10
                0x00FFFF, //Cyan = 11
                0xFF0000, //Red = 12
                0xFF00FF, //Magenta = 13
                0xFFFF00, //Yellow = 14
                0xFFFFFF  //White = 15
            };

            return Color.FromArgb(cColors[(int)Console.ForegroundColor]);
        }

        public static void SetForegroundColor(Color color)
        {
            ConsoleColor ret = 0;
            double rr = color.R, gg = color.G, bb = color.B, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                string n = Enum.GetName(typeof(ConsoleColor), cc);
                Color c = Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                double t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                {
                    Console.ForegroundColor = cc;
                    return;
                }

                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }

            Console.ForegroundColor = ret;
        }
    }
}