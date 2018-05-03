using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugins;
using Rocket.Core.Commands;
using Rocket.Core.Configuration.Json;
using Rocket.Core.Configuration.Xml;
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
            //singleton dependencies
            container.RegisterSingletonType<IEventManager, EventManager>();

            container.RegisterSingletonType<ICommandHandler, DefaultCommandHandler>("default_cmdhandler");
            container.RegisterSingletonType<ICommandHandler, ProxyCommandHandler>("proxy_cmdhandler", null);

            container.RegisterSingletonType<IPluginManager, PluginManager>("default_plugins");
            container.RegisterSingletonType<IPluginManager, ProxyPluginManager>("proxy_plugins", null);

            var pluginManager = (PluginManager) container.Resolve<IPluginManager>("default_plugins");

            container.RegisterSingletonInstance<ICommandProvider>(pluginManager, "plugin_cmdprovider");
            container.RegisterSingletonType<ICommandProvider, RocketCommandProvider>("rocket_cmdprovider");
            container.RegisterSingletonType<ICommandProvider, ProxyCommandProvider>("proxy_cmdprovider", null);

            container.RegisterType<IConfiguration, JsonConfiguration>();
            container.RegisterType<IConfiguration, JsonConfiguration>("json");
            container.RegisterType<IConfiguration, XmlConfiguration>("xml");

            container.RegisterSingletonType<IPermissionProvider, ConfigurationPermissionProvider>(
                "default_permissions");
            container.RegisterSingletonType<IPermissionProvider, ConsolePermissionProvider>("console_permissions");
            container.RegisterSingletonType<IPermissionProvider, ProxyPermissionProvider>("proxy_permissions", null);

            //transient dependencies
            container.RegisterType<ITranslationLocator, TranslationLocator>();
        }
    }
}