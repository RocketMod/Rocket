using Rocket.API;
using Rocket.API.Ioc;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using Rocket.Core.Ioc;
using Rocket.Core.Logging;

namespace Rocket
{
    public class Runtime : IRuntime
    {
        private static Runtime runtime;

        private Runtime()
        {
            Container = new UnityDependencyContainer();
            Container.RegisterInstance<IRuntime>(this);
            Container.RegisterSingletonType<ILogger, ConsoleLogger>();

            Container.Activate(typeof(RegistrationByConvention));
            Container.Get<IImplementation>().Load(this);
            Container.Get<IPluginManager>().Init();
        }

        public IDependencyResolver Resolver { get; private set; }

        public IDependencyContainer Container { get; }

        public static IRuntime Bootstrap() => runtime ?? (runtime = new Runtime());

        public bool IsAlive => runtime != null;
        public string Name => "Rocket.Runtime";
    }
}