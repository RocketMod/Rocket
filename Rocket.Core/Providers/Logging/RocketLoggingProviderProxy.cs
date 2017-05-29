using Rocket.API.Providers.Logging;
using System;
using Rocket.API.Providers;

namespace Rocket.Core.Providers.Logging
{
    [RocketProviderProxy]
    public class RocketLoggingProviderProxy : IRocketLoggingProvider
    {
        public void Invoke(Action<IRocketLoggingProvider> action)
        {
            var providers = R.Providers.GetProviders<IRocketLoggingProvider>();
            foreach (var provider in providers)
                action.Invoke(provider);
        }

        public void Unload(bool isReload = false)
        {
            Invoke(provider => { provider.Unload(isReload); });
        }

        public void Load(bool isReload = false)
        {
            Invoke(provider => { provider.Load(isReload); });
        }

        public void Log(LogLevel level, object message, Exception exception = null)
        {
            Invoke(provider => { provider.Log(level, message, exception); });
        }

        public bool EchoNativeOutput
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public void Log(LogLevel level, Exception exception)
        {
            Log(level, null, exception);
        }
    }
}