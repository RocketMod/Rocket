using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using System.Threading.Tasks;

namespace Rocket.Tests
{
    [TestClass]
    public class Testing
    {
        [AssemblyInitialize()]
        public static void Startup(TestContext testContext)
        {

        }

        private IRuntime runtime;

        [TestInitialize]
        public void Bootstrap()
        {
            runtime = Runtime.Bootstrap();
        }

        [TestMethod]
        public void ImplementationAvailable()
        {
            Assert.IsNotNull(runtime.Container.Get<IImplementation>());
        }

        [TestMethod]
        public void CoreDependenciesAvailable()
        {
            Assert.IsNotNull(runtime.Container.Get<IConfigurationProvider>());
            Assert.IsNotNull(runtime.Container.Get<ITranslationProvider>());
            Assert.IsNotNull(runtime.Container.Get<IPermissionProvider>());
        }


        [TestMethod]
        public void PluginImplementation()
        {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("Test Plugin");
        }

        [TestMethod]
        public async Task PluginEventing()
        {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("Test Plugin");
            bool eventingWorks = await plugin.TestEventing();
            Assert.IsTrue(eventingWorks);
        }
    }
}
