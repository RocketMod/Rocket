using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Logging;
using Rocket.Core.DependencyInjection;
using Rocket.API;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.Plugin;
using Rocket.Core.Eventing;
using Rocket.Core.Plugins;

namespace Rocket
{
    public class Runtime : IRuntime
    {

        private static Runtime runtime = null;
        public static IRuntime Bootstrap()
        {
            if (runtime == null) runtime = new Runtime();
            return runtime;
        }

        public IDependencyContainer Container { get; private set; }

        public IDependencyResolver Resolver { get; private set; }

        public Runtime()
        {
            Container = new UnityDependencyContainer();
            Container.RegisterInstance<IRuntime>(this);
            Container.RegisterSingletonType<ILogger, ConsoleLogger>();
            Container.RegisterSingletonType<IPluginManager, PluginManager>();
            Container.RegisterSingletonType<IEventManager, EventManager>();
            Container.Activate(typeof(RegistrationByConvention));

            IImplementation implementation =  Container.Get<IImplementation>();
            implementation.Load(this);
        }
    }
}
