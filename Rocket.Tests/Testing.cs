using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using Rocket.Core.Configuration.Json;
using Rocket.Core.Extensions;

namespace Rocket.Tests
{
    [TestClass]
    public class Testing
    {
        private IRuntime runtime;

        [AssemblyInitialize]
        public static void Startup(TestContext testContext) { }

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
            Assert.IsNotNull(runtime.Container.Get<IEventManager>());
            Assert.IsNotNull(runtime.Container.Get<IPermissionProvider>());
            Assert.IsNotNull(runtime.Container.Get<IEventManager>());
            Assert.IsNotNull(runtime.Container.Get<ICommandHandler>());
            Assert.IsNotNull(runtime.Container.Get<IPluginManager>());
            Assert.IsNotNull(runtime.Container.Get<ITranslationProvider>());
            Assert.IsNotNull(runtime.Container.Get<IConfigurationProvider>());
        }

        [TestMethod]
        public void PluginImplementation()
        {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin) pluginManager.GetPlugin("TestPlugin");
            Assert.IsTrue(plugin.IsAlive);
        }

        [TestMethod]
        public void TestJsonConfig()
        {
            JsonConfigurationProvider provider = runtime.Container.Get<JsonConfigurationProvider>();
            MemoryStream ms = new MemoryStream();
            ms.Write(
                @"{
	            ""Test1"": ""A""
                ""NestedObjectTest"": {
                   ""NestedStringValue"": ""B"",
                   ""NestedNumberValue"": 4
                }
              }");

            ms.Position = 0;
            IConfigurationRoot config = provider.Load(ms);

            Assert.Equals(config.GetSection("Test1").Value, "B");
            Assert.Equals(config.GetSection("NestedObjectTest").GetSection("NestedStringValue").Value, "B");
            Assert.Equals(config.GetSection("NestedObjectTest").GetSection("NestedNumberValue").Get<int>(), 4);
        }

        /*
        [TestMethod]
        [Ignore]
        public async Task PluginEventing()
        {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("Test Plugin");
            bool eventingWorks = await plugin.TestEventing();
            Assert.IsTrue(eventingWorks);
        }

        [TestMethod]
        [Ignore]
        public async Task PluginEventingWithName()
        {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("Test Plugin");
            bool eventingWorks = await plugin.TestEventingWithName();
            Assert.IsTrue(eventingWorks);
        }
        */
    }
}