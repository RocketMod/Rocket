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
            if (!container.IsRegistered<IEventManager>())
                container.RegisterSingletonType<IEventManager, EventManager>();
            container.RegisterSingletonType<IEventManager, EventManager>("defaulteventmanager");

            if (!container.IsRegistered<ICommandHandler>())
                container.RegisterSingletonType<ICommandHandler, CommandHandler>();
            container.RegisterSingletonType<ICommandHandler, CommandHandler>("defaultcommandhandler");

            if (!container.IsRegistered<IPluginManager>())
                container.RegisterSingletonType<IPluginManager, PluginManager>();
            container.RegisterSingletonType<IPluginManager, PluginManager>("defaultpluginmanager");

            if (!container.IsRegistered<ITranslations>())
                container.RegisterType<ITranslations, Translations>();
            container.RegisterType<ITranslations, Translations>("defaultranslations");

            if (!container.IsRegistered<IPermissionProvider>())
                container.RegisterSingletonType<IPermissionProvider, PermissionProvider>();
            container.RegisterSingletonType<IPermissionProvider, PermissionProvider>("defaultpermissions");

            if (!container.IsRegistered<IConfiguration>())
                container.RegisterSingletonType<IConfiguration, JsonConfiguration>();

            container.RegisterType<IConfiguration, JsonConfiguration>("defaultjson");
        }
    }
}