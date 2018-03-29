using Rocket.API.IOC;
using Rocket.API.Logging;
using Rocket.Core;
using Rocket.IOC;

namespace Rocket
{
    public static class R
    {
        private static DependencyContainer container;
        public static void Bootstrap()
        {
            container = new DependencyContainer();
            container.RegisterSingletonType<ILogger, ConsoleLogger>();
            container.Activate(typeof(Initializer));
        }

        public static IServiceLocator ServiceLocator => container.ServiceLocator;
    }
}
