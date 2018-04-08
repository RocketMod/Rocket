using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using Rocket.Core.Configuration.Json;

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
            Assert.IsNotNull(runtime.Container.Get<ITranslations>());
            Assert.IsNotNull(runtime.Container.Get<IConfiguration>());
            Assert.IsNotNull(runtime.Container.Get<IConfiguration>("defaultjson"));
        }

        [TestMethod]
        public void PluginImplementation()
        {
            IPluginManager pluginManager = runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("TestPlugin");
            Assert.IsTrue(plugin.IsAlive);

            Assert.IsNull(plugin.Configuration); //NoConfig capability
            Assert.IsNull(plugin.Translations); //NoTranslations capability
        }

        [TestMethod]
        public void TestJsonConfig()
        {
            JsonConfiguration config = (JsonConfiguration)runtime.Container.Get<IConfiguration>("defaultjson");

            string json =
            "{" +
                "\"Test1\": \"A\"," +
                "\"NestedObjectTest\": {" +
                   "\"NestedStringValue\": \"B\"," +
                   "\"NestedNumberValue\": 4" +
                "}" +
              "}";

            config.LoadFromJson(json);

            TestConfig(config);

            Assert.ThrowsException<NotSupportedException>(() => config.Save());
        }

        public void TestConfig(IConfiguration config)
        {
            Assert.AreEqual(config.GetSection("Test1").Value, "A");
            Assert.AreEqual(config.GetSection("NestedObjectTest").GetSection("NestedStringValue").Value, "B");
            Assert.AreEqual(config.GetSection("NestedObjectTest").GetSection("NestedNumberValue").Get<int>(), 4);

            Assert.AreEqual(config["Test1"], "A");
            Assert.AreEqual(config["NestedObjectTest.NestedStringValue"], "B");
            Assert.AreEqual(config["NestedObjectTest.NestedNumberValue"], "4");

            Assert.AreEqual(config.GetSection("NestedObjectTest.NestedStringValue").Value, "B");
            Assert.AreEqual(config.GetSection("NestedObjectTest.NestedNumberValue").Get<int>(), 4);
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