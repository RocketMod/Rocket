using Rocket.API.Logging;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Tests
{
    public class TestPlugin : IPlugin
    {
        public IEnumerable<string> Capabilities => new List<string>() { "TESTING" };

        public string Name => "Test Plugin";

        ILogger logger;

        public TestPlugin(ILogger logger)
        {
            this.logger = logger;
        }

        public void Load()
        {
            logger.Info("Hello World (From plugin)");
        }

        public void Unload()
        {
            logger.Info("Bye World (From plugin)");
        }
    }
}
