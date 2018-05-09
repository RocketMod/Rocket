using System;
using Rocket.API.Logging;

namespace Rocket.Tests.Mock
{
    public class NullLogger : ILogger
    {
        public void Log(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                        params object[] arguments) { }

        public bool IsEnabled(LogLevel level) => false;

        public void SetEnabled(LogLevel level, bool enabled) { }
    }
}