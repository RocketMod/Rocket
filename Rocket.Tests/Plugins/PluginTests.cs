using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Plugin;

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

            Assert.IsNull(plugin.Configuration); //NoConfig capability
            Assert.IsNull(plugin.Translations);  //NoTranslations capability
        }
    }
}