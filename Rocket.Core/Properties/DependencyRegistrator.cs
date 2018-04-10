using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using Rocket.Core.Commands;
using Rocket.Core.Configuration.Json;
using Rocket.Core.Eventing;
using Rocket.Core.I18N;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;

namespace Rocket.Core.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            if (!container.IsRegistered<IEventManager>()) container.RegisterSingletonType<IEventManager, EventManager>();
            if (!container.IsRegistered<ICommandHandler>()) container.RegisterSingletonType<ICommandHandler, CommandHandler>();
            if (!container.IsRegistered<IPluginManager>()) container.RegisterSingletonType<IPluginManager, PluginManager>();
            if (!container.IsRegistered<ITranslations>()) container.RegisterType<ITranslations, Translations>();
            if (!container.IsRegistered<IPermissionProvider>()) container.RegisterSingletonType<IPermissionProvider, PermissionProvider>();

            container.RegisterType<IConfiguration, JsonConfiguration>(null, "defaultjson");
        }
    }
}