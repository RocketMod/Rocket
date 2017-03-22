using Rocket.API.Providers;
using System;

namespace Rocket.Core.Providers.Logging
{
    public interface IRocketLoggingProvider : IRocketProviderBase
    {
        void Debug(object message, Exception exception = null);
        void Info(object message, Exception exception = null);
        void Warn(object message, Exception exception = null);
        void Error(object message, Exception exception = null);
        void Fatal(object message, Exception exception = null);
    }
}