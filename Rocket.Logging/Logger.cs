using System;
using log4net;
using log4net.Config;

namespace Rocket.Logging
{
    public static class ObjectExtension
    {
        public static ILog GetLogger(this Object o)
        {
            return LogManager.GetLogger(o.GetType());
        }
    }

    public class Logger
    {
        public Logger()
        {
            BasicConfigurator.Configure();
        }

        public static ILog GetLogger(Type t)
        {
            return LogManager.GetLogger(t);
        }
    }
}
