using System;
using Rocket.API.Logging;

namespace Rocket.Tests.Mock {
    public class NullLogger: ILogger {
        public void Log(string message, LogLevel level = LogLevel.Information, Exception exception = null, ConsoleColor? color = null,
                        params object[] bindings)
        {
        }

        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        public void SetEnabled(LogLevel level, bool enabled)
        {
        }
    }
}