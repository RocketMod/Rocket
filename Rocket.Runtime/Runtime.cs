using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Logging;
using Rocket.Core.DependencyInjection;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using Rocket.Core.Commands;
using Rocket.Core.Eventing;
using Rocket.Core.I18N;
using Rocket.Core.Permissions;
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

        public IDependencyContainer Container { get; }

        public IDependencyResolver Resolver { get; private set; }

        private Runtime()
        {
            Container = new UnityDependencyContainer();
            Container.RegisterInstance<IRuntime>(this);
            Container.RegisterSingletonType<ILogger, ConsoleLogger>();
            Container.RegisterSingletonType<IEventManager, EventManager>();
            Container.RegisterSingletonType<ICommandHandler, CommandHandler>();
            Container.RegisterSingletonType<IPluginManager, PluginManager>();
            Container.RegisterSingletonType<ITranslationProvider, TranslationProvider>();
            Container.RegisterSingletonType<IPermissionProvider, PermissionProvider>();
            Container.Activate(typeof(RegistrationByConvention));
            Container.Get<IImplementation>().Load(this);
            Container.Get<IPluginManager>().Init();
        }
    }
}
