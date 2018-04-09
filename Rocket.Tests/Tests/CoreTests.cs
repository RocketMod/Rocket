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
            TestPlugin plugin = (TestPlugin) pluginManager.GetPlugin("TestPlugin");
            Assert.IsTrue(plugin.IsAlive);

            Assert.IsNull(plugin.Configuration); //NoConfig capability
            Assert.IsNull(plugin.Translations);  //NoTranslations capability
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