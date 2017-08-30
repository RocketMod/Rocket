using System;
using System.Runtime.Serialization;

namespace Rocket.API.Logging
{
    public enum LogLevel { DEBUG, INFO, WARN, ERROR, FATAL };

    [Serializable]
    [DataContract]
    public class LogMessage
    {
        [DataMember]
        public LogLevel LogLevel { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
        public LogMessage()
        {

        }
        public LogMessage(LogLevel logLevel, object message, Exception exception = null)
        {
            LogLevel = logLevel;
            Message = message?.ToString();
            Exception = exception;
        }
    }
}
