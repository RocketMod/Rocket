using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Logging;
using Rocket.Core.Logging;
using Rocket.Tests.Mock;

namespace Rocket.Tests
{
    public abstract class RocketTestBase
    {
        protected IRuntime Runtime { get; private set; }

        [AssemblyInitialize]
        public static void Startup(TestContext testContext) { }

        [TestInitialize]
        public virtual void Bootstrap()
        {
            Runtime = Rocket.Runtime.Bootstrap();
            Runtime.Container.RegisterSingletonType<ILogger, NullLogger>("default_file_logger");
        }
    }
}