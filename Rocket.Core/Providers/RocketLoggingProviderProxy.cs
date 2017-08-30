
using System;
using Rocket.API.Providers;
using Rocket.API.Logging;

namespace Rocket.Core.Providers.Logging
{
    public class RocketLoggingProviderProxy : ProxyBase<ILoggingProvider>, ILoggingProvider
    {
        public void Log(LogLevel level, object message, Exception exception = null, ConsoleColor? color = null)
        {
            InvokeAll(provider => { provider.Log(level, message, exception, color); });
        }

        public bool EchoNativeOutput
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public void Log(LogLevel level, Exception exception, ConsoleColor? color = null)
        {
            InvokeAll(provider => { provider.Log(level, exception, color); });
        }

        public void LogMessage(LogLevel level, object message, ConsoleColor? color = null)
        {
            InvokeAll(provider => { provider.LogMessage(level, message, color); });
        }

    }
}