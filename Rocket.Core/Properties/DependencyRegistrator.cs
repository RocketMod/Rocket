using System.ComponentModel;
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
            container.RegisterSingletonType<ILogger, ProxyLogger>("proxy_logger", null);

            container.RegisterSingletonType<IEventManager, EventManager>();

            container.RegisterSingletonType<ICommandHandler, DefaultCommandHandler>("default_cmdhandler");
            container.RegisterSingletonType<ICommandHandler, ProxyCommandHandler>("proxy_cmdhandler", null);

            container.RegisterSingletonType<IPluginManager, PluginManager>("default_plugins");
            container.RegisterSingletonType<IPluginManager, ProxyPluginManager>("proxy_plugins", null);

            container.RegisterSingletonType<ICommandProvider, RocketCommandProvider>("rocket_cmdprovider");
            container.RegisterSingletonType<ICommandProvider, ProxyCommandProvider>("proxy_cmdprovider", null);

            container.RegisterSingletonType<IPermissionProvider, ConfigurationPermissionProvider>(
                "default_permissions");
            container.RegisterSingletonType<IPermissionProvider, ConsolePermissionProvider>("console_permissions");
            container.RegisterSingletonType<IPermissionProvider, ProxyPermissionProvider>("proxy_permissions", null);

            container.RegisterType<ITranslationCollection, TranslationCollection>();

            container.RegisterSingletonType<IUserManager, ProxyUserManager>();
        }
    }
}