using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Plugins;
using Rocket.Core.Plugins;
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
            IPluginLoader pluginLoader = Runtime.Container.Resolve<IPluginLoader>();
            TestPlugin plugin = (TestPlugin) (pluginLoader).GetPlugin("TestPlugin");
            Assert.IsTrue(plugin.IsAlive);

            Assert.IsNull(plugin.Configuration); //No config for test plugin
            Assert.IsNull(plugin.Translations);  //No translations for test plugin
        }
    }
}