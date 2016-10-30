using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Logging
{
    [Serializable]
    public class LogMessage
    {
        public LogLevel LogLevel { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
        public LogMessage()
        {

        }
        public LogMessage(LogLevel logLevel, object message, Exception exception = null)
        {
            LogLevel = logLevel;
            Message = message.ToString();
            Exception = exception;
        }
    }
}
