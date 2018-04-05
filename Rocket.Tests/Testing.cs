using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;

namespace Rocket.Tests {
    [TestClass]
    public class Testing
    {
        private IRuntime runtime;

        [AssemblyInitialize]
        public static void Startup(TestContext testContext) { }

        [TestInitialize]
        public void Bootstrap() {
            runtime = Runtime.Bootstrap();
        }

        [TestMethod]
        public void ImplementationAvailable() {
            Assert.IsNotNull(runtime.Container.Get<IImplementation>());
        }

        [TestMethod]
        public void CoreDependenciesAvailable() {
            Assert.IsNotNull(runtime.Container.Get<IConfiguration>());
            Assert.IsNotNull(runtime.Container.Get<ITranslationProvider>());
            Assert.IsNotNull(runtime.Container.Get<IPermissionProvider>());
        }


        [TestMethod]
        public void PluginImplementation() {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin) pluginManager.GetPlugin("Test Plugin");
            Assert.IsTrue(plugin.IsAlive);
        }

        [TestMethod]
        [Ignore]
        public async Task PluginEventing() {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin) pluginManager.GetPlugin("Test Plugin");
            bool eventingWorks = await plugin.TestEventing();
            Assert.IsTrue(eventingWorks);
        }

        [TestMethod]
        [Ignore]
        public async Task PluginEventingWithName() {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin) pluginManager.GetPlugin("Test Plugin");
            bool eventingWorks = await plugin.TestEventingWithName();
            Assert.IsTrue(eventingWorks);
        }
    }
}