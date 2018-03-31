using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            IRuntime runtime = Runtime.Bootstrap();
        }

        [TestMethod]
        public void ImplementationAvailable()
        {
            Assert.IsNotNull(runtime.Container.GetInstance<IImplementation>());
        }

        [TestMethod]
        public void CoreDependenciesAvailable()
        {
            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<IConfigurationProvider>());
            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<ITranslationProvider>());
            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<IPermissionProvider>());
        }

        [TestMethod]
        public void PluginImplementation()
        {
            IPluginManager pluginManager = Runtime.ServiceLocator.GetInstance<IPluginManager>();
            TestPlugin plugin = (TestPlugin)pluginManager.GetPlugin("Test Plugin");
            Assert.IsTrue(plugin.Loaded); 
        }
    }
}
