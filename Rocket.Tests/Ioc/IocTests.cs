using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;

namespace Rocket.Tests.Ioc
{
    [TestClass]
    [TestCategory("Ioc")]
    public class IocTests : RocketTestBase
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
            Assert.IsNotNull(Runtime.Container.Get<IPermissionProvider>());
            Assert.IsNotNull(Runtime.Container.Get<IEventManager>());
            Assert.IsNotNull(Runtime.Container.Get<ICommandHandler>());
            Assert.IsNotNull(Runtime.Container.Get<IPluginManager>());
            Assert.IsNotNull(Runtime.Container.Get<ITranslations>());
            Assert.IsNotNull(Runtime.Container.Get<IConfiguration>());
            Assert.IsNotNull(Runtime.Container.Get<IConfiguration>("defaultjson"));
        }
    }
}