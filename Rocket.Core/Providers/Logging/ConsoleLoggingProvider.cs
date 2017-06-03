using System;
using Rocket.API.Providers;
using Rocket.API.Providers.Logging;

namespace Rocket.Core.Providers.Logging
{
    [RocketProviderImplementation(true)]
    public class ConsoleLoggingProvider : IRocketLoggingProvider
    {
        public void Log(LogLevel level, object message, Exception exception = null)
        {
            string msg = (message ?? "") + (exception?.ToString() ?? "");

            if (message == null)
            {
                msg = exception?.ToString();
            }

            Console.WriteLine($"[{level}] {msg ?? "null"}");
        }

        public void Unload(bool isReload = false)
        {
            //
        }

        public void Load(bool isReload = false)
        {
            //
        }

        public void Log(LogLevel level, Exception exception)
        {
            Log(level, null, exception);
        }

        public bool EchoNativeOutput { get; } = false;
    }
}