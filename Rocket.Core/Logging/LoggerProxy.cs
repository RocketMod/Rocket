using System;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Logging
{
    public class LoggerProxy : ServiceProxy<ILogger>, ILogger
    {
        public LoggerProxy(IDependencyContainer container) : base(container) { }

        public void Log(LogLevel level, object message, Exception exception = null, params object[] arguments)
        {
            foreach (ILogger service in ProxiedServices)
            {
                service.Log(level, message, exception, arguments);
            }
        }

        public bool IsEnabled(LogLevel level)
        {
            return ProxiedServices.Any(c => c.IsEnabled(level));
        }

        public void SetEnabled(LogLevel level, bool enabled)
        {
            throw new NotSupportedException("Not supported on proxy provider.");
        }

        public string ServiceName => "LoggerProxy";
    }
}