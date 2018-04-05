using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using Rocket.Core.Commands;
using Rocket.Core.Configuration.Json;
using Rocket.Core.Eventing;
using Rocket.Core.I18N;
using Rocket.Core.Logging;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;

namespace Rocket.Core.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IEventManager, EventManager>();
            container.RegisterSingletonType<IPermissionProvider, PermissionProvider>();
            container.RegisterSingletonType<ICommandHandler, CommandHandler>();
            container.RegisterSingletonType<IPluginManager, PluginManager>();
            container.RegisterSingletonType<ITranslationProvider, TranslationProvider>();
            container.RegisterSingletonType<IPermissionProvider, PermissionProvider>();
            container.RegisterSingletonType<IConfigurationProvider, JsonConfigurationProvider>("json");
        }
    }
}