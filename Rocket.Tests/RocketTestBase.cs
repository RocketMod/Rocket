using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;

namespace Rocket.Tests
{
    public abstract class RocketTestBase
    {
        protected IRuntime Runtime { get; private set; }

        [AssemblyInitialize]
        public static void Startup(TestContext testContext) { }

        [TestInitialize]
        public void Bootstrap()
        {
            Runtime = Rocket.Runtime.Bootstrap();
        }
    }
}