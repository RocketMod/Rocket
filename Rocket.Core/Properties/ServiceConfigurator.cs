using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.API.Plugins;
using Rocket.API.Scheduling;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.Configuration;
using Rocket.Core.Configuration.Json;
using Rocket.Core.Configuration.Xml;
using Rocket.Core.Configuration.Yaml;
using Rocket.Core.Eventing;
using Rocket.Core.I18N;
using Rocket.Core.Logging;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;
using Rocket.Core.Plugins.NuGet;
using Rocket.Core.Scheduling;
using Rocket.Core.User;

namespace Rocket.Core.Properties
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IDependencyContainer container)
        {
            container.AddSingleton<ITaskScheduler, DefaultTaskScheduler>();
            container.AddTransient<IConfiguration, YamlConfiguration>("yaml", null);
            container.AddTransient<IConfiguration, JsonConfiguration>("json");
            container.AddTransient<IConfiguration, XmlConfiguration>("xml");

            container.AddSingleton<IRocketConfigurationProvider, RocketConfigurationProvider>();

            container.AddSingleton<ILogger, ConsoleLogger>("console_logger");
            container.AddSingleton<ILogger, LoggerProxy>("proxy_logger", null);

            container.AddSingleton<IEventBus, EventBus>();

            container.AddSingleton<ICommandHandler, DefaultCommandHandler>("default_cmdhandler");
            container.AddSingleton<ICommandHandler, CommandHandlerProxy>("proxy_cmdhandler", null);

            container.AddSingleton<IPluginLoader, DefaultClrPluginLoader>("dll_plugins");
            container.AddSingleton<IPluginLoader, NuGetPluginLoader>("nuget_plugins");
            container.AddSingleton<IPluginLoader, PluginLoaderProxy>("proxy_plugins", null);

            container.AddSingleton<ICommandProvider, RocketCommandProvider>("rocket_cmdprovider");
            container.AddSingleton<ICommandProvider, CommandProviderProxy>("proxy_cmdprovider", null);

            var configPermissions = container.Activate<ConfigurationPermissionProvider>();
            container.AddTransient<IPermissionProvider>(configPermissions, "default_permissions", null);
            container.AddTransient<IPermissionChecker>(configPermissions, "default_permissions");

            container.AddSingleton<IPermissionChecker, ConsolePermissionChecker>("console_checker");
            container.AddSingleton<IPermissionChecker, PermissionProviderProxy>("proxy_checker", null);

            container.AddTransient<ITranslationCollection, TranslationCollection>();
            container.AddSingleton<IUserManager, UserManagerProxy>();
        }
    }
}