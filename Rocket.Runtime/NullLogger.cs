using System;
using Rocket.API.Logging;

namespace Rocket
{
    public class NullLogger : ILogger
    {
        public void Log(LogLevel level, object message, Exception exception = null, params object[] arguments)
        { }

        public bool IsEnabled(LogLevel level) => false;

        public void SetEnabled(LogLevel level, bool enabled) { }

        public string ServiceName => "NullLogger";
    }
}