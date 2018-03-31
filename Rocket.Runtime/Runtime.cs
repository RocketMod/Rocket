using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Logging;
using Rocket.Core.DependencyInjection;

namespace Rocket
{
    public static class Runtime
    {
        private static IDependencyContainer container;

        public static void Bootstrap()
        {
            container = new UnityDependencyContainer();
            container.RegisterSingletonType<ILogger, ConsoleLogger>();
            container.Activate(typeof(RegistrationByConvention));
        }

        public static IServiceLocator ServiceLocator => container.ServiceLocator;
    }
}
