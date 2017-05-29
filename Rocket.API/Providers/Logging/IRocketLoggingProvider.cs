using System;
using UnityEngine;

namespace Rocket.API.Providers.Logging
{
    [RocketProvider]
    public interface IRocketLoggingProvider : IRocketProviderBase
    {
        bool EchoNativeOutput { get; }
        void Log(LogLevel level, object message, Exception exception = null);
        void Log(LogLevel level, Exception exception);
    }
}