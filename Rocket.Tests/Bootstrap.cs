using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Tests
{
    [TestClass]
    public class Bootstrap
    {
        [TestInitialize()]
        public void Startup()
        {
            Runtime.Bootstrap();
        }

        [TestMethod]
        public void CheckImplementation()
        {
            IImplementation implementation = Runtime.ServiceLocator.GetInstance<IImplementation>();
            Assert.IsNotNull(implementation);
        }
    }
}
