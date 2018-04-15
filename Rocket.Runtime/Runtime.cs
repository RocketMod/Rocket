using System;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Logging;

namespace Rocket
{
    public class Runtime : IRuntime
    {
        private Runtime()
        {
            Container = new UnityDependencyContainer();
            Container.RegisterInstance<IRuntime>(this);
            Container.RegisterSingletonType<ILogger, ConsoleLogger>();

            Container.Activate(typeof(RegistrationByConvention));
            Container.Get<IImplementation>().Init(this);
        }

        public IDependencyResolver Resolver { get; private set; }

        public IDependencyContainer Container { get; }

        public static IRuntime Bootstrap() => new Runtime();

        public bool IsAlive => true;
        public string Name => "Rocket.Runtime";
        public string WorkingDirectory { get; } = Environment.CurrentDirectory;
        public string ConfigurationName { get; } = "Rocket";
    }
}