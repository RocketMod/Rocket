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
        public static IRuntime Bootstrap(IDependencyContainer container)
        {
            var runtime = new Runtime(container);
            runtime.Init();
            return runtime;
        }

        public IDependencyContainer Container { get; }

        public IDependencyResolver Resolver { get; private set; }

        private Runtime(IDependencyContainer container)
        {
            Container = container;
        }

        private void Init()
        {
            Container.RegisterInstance<IRuntime>(this);
            Container.RegisterSingletonType<ILogger, ConsoleLogger>();
            Container.RegisterSingletonType<IEventManager, EventManager>();
            Container.RegisterSingletonType<IPluginManager, PluginManager>();
            Container.Activate(typeof(RegistrationByConvention));

            IImplementation implementation = Container.Get<IImplementation>();
            implementation.Load(this);
        }
    }
}
