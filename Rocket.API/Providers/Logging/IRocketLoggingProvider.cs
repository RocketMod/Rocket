using System;
using UnityEngine;

namespace Rocket.API.Providers.Logging
{
    [ProviderDefinition]
    public interface IRocketLoggingProvider
    {
        void LogMessage(LogLevel level, object message, ConsoleColor? color = null);
        void Log(LogLevel level, object message, Exception exception = null, ConsoleColor? color = null);
        void Log(LogLevel level, Exception exception, ConsoleColor? color = null);
    }
}