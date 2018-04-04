using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using Rocket.Core.Eventing;
using Rocket.Core.I18N;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;

namespace Rocket.Core.Properties {
    public class DependencyRegistrator : IDependencyRegistrator {
        public void Register(IDependencyContainer container, IDependencyResolver resolver) {
            container.RegisterSingletonType<IEventManager, EventManager>();
            container.RegisterSingletonType<IConfigurationProvider, ConfigurationProvider>();
            container.RegisterSingletonType<IPermissionProvider, PermissionProvider>();
            container.RegisterSingletonType<ITranslationProvider, TranslationProvider>();

            container.RegisterSingletonType<IPluginManager, PluginManager>();
        }
    }
}