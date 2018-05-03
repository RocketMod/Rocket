using System;
using System.Diagnostics;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Permissions;
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
            Container.RegisterSingletonType<ILogger, ConsoleLogger>("console_logger");
            Container.RegisterSingletonType<ILogger, ProxyLogger>("proxy_logger", null);

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(typeof(Runtime).Assembly.Location);
            Container.Resolve<ILogger>()
                     .LogInformation("Initializing RocketMod " + versionInfo.FileVersion, ConsoleColor.DarkGreen);

            Container.Activate(typeof(RegistrationByConvention));

            var permissions = Container.Resolve<IPermissionProvider>();
            var impl = Container.Resolve<IImplementation>();
           
            permissions.Load(this);
            impl.Init(this);
        }

        public IDependencyResolver Resolver { get; private set; }

        public IDependencyContainer Container { get; }

        public bool IsAlive => true;
        public string Name => "Rocket.Runtime";
        public string WorkingDirectory => Container.Resolve<IImplementation>().WorkingDirectory;
        public string ConfigurationName { get; } = "Rocket";

        public static IRuntime Bootstrap() => new Runtime();
    }
}