using Rocket.API.Providers.Logging;
using System;

namespace Rocket.Core.Providers.Logging
{
    public class RocketLoggingProviderProxy : IRocketLoggingProvider
    {
        public delegate void Log(LogMessage message);
        public event Log OnLog;

        public void Invoke(Action<IRocketLoggingProvider> action)
        {
            foreach (var provider in R.Providers.GetProviders<IRocketLoggingProvider>())
                action.Invoke(provider);
        }

        public void Debug(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.DEBUG, message, exception));
            Invoke((IRocketLoggingProvider provider) => { provider.Debug(message, exception); });
        }

        public void Info(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.INFO, message, exception));
            Invoke((IRocketLoggingProvider provider) => { provider.Debug(message, exception); });
        }

        public void Warn(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.WARN, message, exception));
            Invoke((IRocketLoggingProvider provider) => { provider.Debug(message, exception); });
        }

        public void Error(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.ERROR, message, exception));
            Invoke((IRocketLoggingProvider provider) => { provider.Debug(message, exception); });
        }

        public void Fatal(object message, Exception exception = null)
        {
            OnLog?.Invoke(new LogMessage(LogLevel.FATAL, message, exception));
            Invoke((IRocketLoggingProvider provider) => { provider.Debug(message, exception); });
        }

        public void Unload()
        {
            Invoke((IRocketLoggingProvider provider) => { provider.Unload(); });
        }

        public void Load(bool isReload = false)
        {
            Invoke((IRocketLoggingProvider provider) => { provider.Load(isReload); });
        }
    }
}
