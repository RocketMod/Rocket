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
        public static IRuntime Bootstrap(IDependencyContainer container)
        {
            var runtime = new Runtime(container);
            runtime.Init();
            return runtime;
        }

        public IDependencyContainer Container { get; }

        private Runtime(IDependencyContainer container)
        {
            Container = container;
        }

        private void Init()
        {
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
