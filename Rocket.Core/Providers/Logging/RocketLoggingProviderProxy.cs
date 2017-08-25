using Rocket.API.Providers.Logging;
using System;
using Rocket.API.Providers;

namespace Rocket.Core.Providers.Logging
{
    [ProviderProxy]
    public class RocketLoggingProviderProxy : IRocketLoggingProvider
    {
        public void Invoke(Action<IRocketLoggingProvider> action)
        {
            var providers = R.Providers.GetProviders<IRocketLoggingProvider>();
            foreach (var provider in providers)
                action?.Invoke(provider);
        }

        public void Unload(bool isReload = false)
        {
            Invoke(provider => { provider.Unload(isReload); });
        }

        public void Load(bool isReload = false)
        {
            Invoke(provider => { provider.Load(isReload); });
        }

        public void Log(LogLevel level, object message, Exception exception = null, ConsoleColor? color = null)
        {
            Invoke(provider => { provider.Log(level, message, exception, color); });
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
            Invoke(provider => { provider.Log(level, exception, color); });
        }

        public void LogMessage(LogLevel level, object message, ConsoleColor? color = null)
        {
            Invoke(provider => { provider.LogMessage(level, message, color); });
        }
    }
}