using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Tests
{
    [TestClass]
    public class Testing
    {
        [TestInitialize()]
        public void Startup()
        {
            Runtime.Bootstrap();
        }

        [TestMethod]
        public void ImplementationAvailable()
        {
            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<IImplementation>());
        }

        [TestMethod]
        public void CoreDependenciesAvailable()
        {
            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<IConfigurationProvider>());
            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<ITranslationProvider>());
            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<IPermissionProvider>());

            Assert.IsNotNull(Runtime.ServiceLocator.GetInstance<IPluginManager>());
        }
    }
}
