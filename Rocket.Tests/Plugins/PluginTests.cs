using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Plugin;
using Rocket.Tests.Mock;

namespace Rocket.Tests.Plugins
{
    [TestClass]
    [TestCategory("Plugins")]
    public class PluginTests : RocketTestBase
    {
        [TestMethod]
        public void PluginImplementation()
        {
            IPluginManager pluginManager = Runtime.Container.Get<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("TestPlugin");
            Assert.IsTrue(plugin.IsAlive);

            Assert.IsNull(plugin.Configuration); //No config for test plugin
            Assert.IsNull(plugin.Translations);  //No translations for test plugin
        }
    }
}