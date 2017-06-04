using System;
using UnityEngine;

namespace Rocket.API.Providers.Logging
{
    [RocketProvider]
    public interface IRocketLoggingProvider : IRocketProviderBase
    {
        bool EchoNativeOutput { get; }
        void LogMessage(LogLevel level, object message, ConsoleColor? color = null);
        void Log(LogLevel level, object message, Exception exception = null, ConsoleColor? color = null);
        void Log(LogLevel level, Exception exception, ConsoleColor? color = null);
    }
}