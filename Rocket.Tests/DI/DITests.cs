using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;

namespace Rocket.Tests.DI
{
    [TestClass]
    [TestCategory("Dependency Injection")]
    public class DITests : RocketTestBase
    {
        [TestMethod]
        public void ImplementationAvailable()
        {
            Assert.IsNotNull(Runtime.Container.Get<IImplementation>());
        }

        [TestMethod]
        public void CoreDependenciesAvailable()
        {
            Assert.IsNotNull(Runtime.Container.Get<IEventManager>());
            Assert.IsNotNull(Runtime.Container.Get<ITranslationLocator>());

            Assert.IsNotNull(Runtime.Container.Get<IPermissionProvider>());
            Assert.IsNotNull(Runtime.Container.Get<IPermissionProvider>("proxy_permissions"));
            Assert.IsNotNull(Runtime.Container.Get<IPermissionProvider>("config_permissions"));
            Assert.IsNotNull(Runtime.Container.Get<IPermissionProvider>("console_permissions"));

            Assert.IsNotNull(Runtime.Container.Get<ICommandHandler>());
            Assert.IsNotNull(Runtime.Container.Get<ICommandHandler>("proxy_cmdhandler"));
            Assert.IsNotNull(Runtime.Container.Get<ICommandHandler>("default_cmdhandler"));

            Assert.IsNotNull(Runtime.Container.Get<IPluginManager>());
            Assert.IsNotNull(Runtime.Container.Get<IPluginManager>("default_plugins"));
            Assert.IsNotNull(Runtime.Container.Get<IPluginManager>("proxy_plugins"));

            Assert.IsNotNull(Runtime.Container.Get<IConfiguration>());
            Assert.IsNotNull(Runtime.Container.Get<IConfiguration>("json"));
            Assert.IsNotNull(Runtime.Container.Get<IConfiguration>("xml"));
        }
    }
}