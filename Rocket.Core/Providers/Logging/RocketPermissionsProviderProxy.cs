using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Providers.Logging
{
    public class RocketLoggingProviderProxy
    {
        public delegate void Log(LogMessage message);
        public event Log OnLog;

        public void Invoke(Action<IRocketLoggingProvider> action)
        {
            foreach (var provider in R.GetProviders<IRocketLoggingProvider>())
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


    }
}
