using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.Configuration;
using Rocket.Core.Configuration.Json;
using Rocket.Core.Configuration.Xml;
using Rocket.Core.Eventing;
using Rocket.Core.I18N;
using Rocket.Core.Logging;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;
using Rocket.Core.Plugins.NuGet;
using Rocket.Core.User;

namespace Rocket.Core.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterType<IConfiguration, JsonConfiguration>();
            container.RegisterType<IConfiguration, JsonConfiguration>("json");
            container.RegisterType<IConfiguration, XmlConfiguration>("xml");

            container.RegisterSingletonType<IRocketSettingsProvider, RocketSettingsProvider>();

            container.RegisterSingletonType<ILogger, ConsoleLogger>("console_logger");
            container.RegisterSingletonType<ILogger, LoggerProxy>("proxy_logger", null);

            container.RegisterSingletonType<IEventBus, EventBus>();

            container.RegisterSingletonType<ICommandHandler, DefaultCommandHandler>("default_cmdhandler");
            container.RegisterSingletonType<ICommandHandler, CommandHandlerProxy>("proxy_cmdhandler", null);

            container.RegisterSingletonType<IPluginLoader, DefaultClrPluginLoader>("dll_plugins");
            container.RegisterSingletonType<IPluginLoader, NuGetPluginLoader>("nuget_plugins");
            container.RegisterSingletonType<IPluginLoader, PluginLoaderProxy>("proxy_plugins", null);

            container.RegisterSingletonType<ICommandProvider, RocketCommandProvider>("rocket_cmdprovider");
            container.RegisterSingletonType<ICommandProvider, CommandProviderProxy>("proxy_cmdprovider", null);

            container.RegisterSingletonType<IPermissionProvider, ConfigurationPermissionProvider>("default_permissions");
            container.RegisterSingletonType<IPermissionProvider, ConsolePermissionProvider>("console_permissions");
            container.RegisterSingletonType<IPermissionProvider, PermissionProviderProxy>("proxy_permissions", null);

            container.RegisterType<ITranslationCollection, TranslationCollection>();

            container.RegisterSingletonType<IUserManager, UserManagerProxy>();
        }
    }
}