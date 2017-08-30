using System;
using Rocket.API.Providers;
using Rocket.API.Logging;

namespace Rocket.Core.Providers.Logging
{
    public class ConsoleLoggingProvider : ProviderBase, ILoggingProvider
    {

        public bool EchoNativeOutput { get; } = false;
        public void LogMessage(LogLevel level, object message, ConsoleColor? color = null)
        {
            Log(level, message, null, color);
        }

        public void Log(LogLevel level, object message, Exception exception = null, ConsoleColor? color = null)
        {
            if(color == null) color = ConsoleColor.Gray;
            var col = Console.ForegroundColor;
            Console.ForegroundColor = color.Value;
            string msg = (message ?? "") + (exception?.ToString() ?? "");

            if (message == null)
            {
                msg = exception?.ToString();
            }

            Console.WriteLine($"[{level}] {msg ?? "null"}");
            Console.ForegroundColor = col;
        }

        public void Log(LogLevel level, Exception exception, ConsoleColor? color = null)
        {
            Log(level, null, exception, color);
        }

        protected override void OnLoad(ProviderManager providerManager)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }
    }
}