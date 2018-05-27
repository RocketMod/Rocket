using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugins;

namespace Rocket.Tests.DI
{
    [TestClass]
    [TestCategory("Dependency Injection")]
    public class DITests : RocketTestBase
    {
        [TestMethod]
        public void ImplementationAvailable()
        {
            Assert.IsNotNull(Runtime.Container.Resolve<IHost>());
        }

        [TestMethod]
        public void CoreDependenciesAvailable()
        {
            Assert.IsNotNull(Runtime.Container.Resolve<IEventManager>());
            Assert.IsNotNull(Runtime.Container.Resolve<ITranslationCollection>());

            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionProvider>());
            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionProvider>("proxy_permissions"));
            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionProvider>("default_permissions"));
            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionProvider>("console_permissions"));

            Assert.IsNotNull(Runtime.Container.Resolve<ICommandHandler>());
            Assert.IsNotNull(Runtime.Container.Resolve<ICommandHandler>("proxy_cmdhandler"));
            Assert.IsNotNull(Runtime.Container.Resolve<ICommandHandler>("default_cmdhandler"));

            Assert.IsNotNull(Runtime.Container.Resolve<IPluginManager>());
            Assert.IsNotNull(Runtime.Container.Resolve<IPluginManager>("dll_plugins"));
            Assert.IsNotNull(Runtime.Container.Resolve<IPluginManager>("nuget_plugins"));
            Assert.IsNotNull(Runtime.Container.Resolve<IPluginManager>("proxy_plugins"));

            Assert.IsNotNull(Runtime.Container.Resolve<IConfiguration>());
            Assert.IsNotNull(Runtime.Container.Resolve<IConfiguration>("json"));
            Assert.IsNotNull(Runtime.Container.Resolve<IConfiguration>("xml"));
        }
    }
}