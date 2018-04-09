using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Logging;

namespace Rocket.Tests
{
    public class TestImplementation : IImplementation
    {
        private readonly ILogger logger;

        public TestImplementation(ILogger logger)
        {
            this.logger = logger;
        }

        public IEnumerable<string> Capabilities => new List<string>
        {
            "TESTING"
        };

        public string InstanceId => "Test Instance";

        public bool IsAlive => throw new NotImplementedException();

        public void Load(IRuntime runtime)
        {
            logger.LogInformation("Loading implementation");
        }

        public void Reload()
        {
            logger.LogInformation("Reloading implementation");
        }

        public void Shutdown()
        {
            logger.LogInformation("Shutting down implementation");
        }

        public string Name => "TestImplementation";
    }
}