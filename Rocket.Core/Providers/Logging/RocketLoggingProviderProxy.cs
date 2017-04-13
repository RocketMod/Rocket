using Rocket.API.Providers.Logging;
using System;
using Rocket.API.Providers;

namespace Rocket.Core.Providers.Logging
{
    [RocketProviderProxy]
    public class RocketLoggingProviderProxy : IRocketLoggingProvider
    {
        public event Action<LogMessage> OnLog;

        public void Invoke(Action<IRocketLoggingProvider> action)
        {
            foreach (var provider in R.Providers.GetProviders<IRocketLoggingProvider>())
                action.Invoke(provider);
        }

        public void Debug(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.DEBUG, message, exception));
            Invoke(provider => { provider.Debug(message, exception); });
        }

        public void Info(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.INFO, message, exception));
            Invoke(provider => { provider.Debug(message, exception); });
        }

        public void Warn(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.WARN, message, exception));
            Invoke(provider => { provider.Debug(message, exception); });
        }

        public void Error(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.ERROR, message, exception));
            Invoke(provider => { provider.Debug(message, exception); });
        }

        public void Fatal(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.FATAL, message, exception));
            Invoke(provider => { provider.Debug(message, exception); });
        }

        public void Unload()
        {
            Invoke(provider => { provider.Unload(); });
        }

        public void Load(bool isReload = false)
        {
            Invoke(provider => { provider.Load(isReload); });
        }
    }
}
