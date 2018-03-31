using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Logging;

namespace Rocket.Tests
{
    public class Implementation : IImplementation
    {
        ILogger logger;
        public Implementation(ILogger logger)
        {
            this.logger = logger;
        }

        public IEnumerable<string> Capabilities => new List<string>() { "TESTING" };

        public string InstanceId => "Test Instance";

        public void Load(IRuntime runtime)
        {
            logger.Info("Loading implementation");
        }

        public void Reload()
        {
            logger.Info("Reloading implementation");
        }

        public void Shutdown()
        {
            logger.Info("Shutting down implementation");
        }
    }
}
