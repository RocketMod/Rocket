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

namespace Rocket.Tests.Tests
{
    [TestClass]
    public class CoreTests : RocketTestBase
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

        [TestMethod]
        public void PluginImplementation()
        {
            IPluginManager pluginManager = Runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("TestPlugin");
            Assert.IsTrue(plugin.IsAlive);

            Assert.IsNull(plugin.Configuration); //NoConfig capability
            Assert.IsNull(plugin.Translations); //NoTranslations capability
        }

        [TestMethod]
        public void TestJsonConfig()
        {
            JsonConfiguration config = (JsonConfiguration)Runtime.Container.Get<IConfiguration>("defaultjson");

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

        [TestMethod]
        public void TestJsonSetObjectConfig()
        {
            JsonConfiguration config = (JsonConfiguration)Runtime.Container.Get<IConfiguration>("defaultjson");

            config.LoadEmpty();

            var @object = new
            {
                Test1 = "A",
                NestedObjectTest = new
                {
                    NestedStringValue = "B",
                    NestedNumberValue = 4
                }
            };

            config.Set(@object);
            TestConfig(config);

            Assert.ThrowsException<NotSupportedException>(() => config.Save());
        }

        public void TestConfig(IConfiguration config)
        {
            Assert.AreEqual(config.GetSection("Test1").Get<string>(), "A");
            Assert.AreEqual(config.GetSection("NestedObjectTest").GetSection("NestedStringValue").Get<string>(), "B");
            Assert.AreEqual(config.GetSection("NestedObjectTest").GetSection("NestedNumberValue").Get<int>(), 4);

            Assert.AreEqual(config["Test1"].Get<string>(), "A");
            Assert.AreEqual(config["NestedObjectTest.NestedStringValue"].Get<string>(), "B");
            Assert.AreEqual(config["NestedObjectTest.NestedNumberValue"].Get<int>(), 4);

            Assert.AreEqual(config["NestedObjectTest.NestedNumberValue"].Key, "NestedNumberValue");
            Assert.AreEqual(config["NestedObjectTest.NestedNumberValue"].Path, "NestedObjectTest.NestedNumberValue");

            Assert.AreEqual(config["NestedObjectTest"]["NestedStringValue"].Get<string>(), "B");
            Assert.AreEqual(config["NestedObjectTest"]["NestedNumberValue"].Get<int>(), 4);

            Assert.AreEqual(config.GetSection("NestedObjectTest.NestedStringValue").Get<string>(), "B");
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