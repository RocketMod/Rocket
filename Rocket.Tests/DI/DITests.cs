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
            Assert.IsNotNull(Runtime.Container.Resolve<IEventBus>());
            Assert.IsNotNull(Runtime.Container.Resolve<ITranslationCollection>());

            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionProvider>());
            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionProvider>("default_permissions"));

            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionChecker>());
            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionChecker>("proxy_checker"));
            Assert.IsNotNull(Runtime.Container.Resolve<IPermissionChecker>("console_checker"));

            Assert.IsNotNull(Runtime.Container.Resolve<ICommandHandler>());
            Assert.IsNotNull(Runtime.Container.Resolve<ICommandHandler>("proxy_cmdhandler"));
            Assert.IsNotNull(Runtime.Container.Resolve<ICommandHandler>("default_cmdhandler"));

            Assert.IsNotNull(Runtime.Container.Resolve<IPluginLoader>());
            Assert.IsNotNull(Runtime.Container.Resolve<IPluginLoader>("dll_plugins"));
            Assert.IsNotNull(Runtime.Container.Resolve<IPluginLoader>("nuget_plugins"));
            Assert.IsNotNull(Runtime.Container.Resolve<IPluginLoader>("proxy_plugins"));

            Assert.IsNotNull(Runtime.Container.Resolve<IConfiguration>());
            Assert.IsNotNull(Runtime.Container.Resolve<IConfiguration>("yaml"));
            Assert.IsNotNull(Runtime.Container.Resolve<IConfiguration>("json"));
            Assert.IsNotNull(Runtime.Container.Resolve<IConfiguration>("xml"));
        }
    }
}