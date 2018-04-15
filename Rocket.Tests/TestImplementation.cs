using Rocket.API;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.Tests
{
    public class TestImplementation : IImplementation
    {
        private readonly ILogger logger;
        private readonly IConsoleCommandCaller consoleCaller;

        public TestImplementation(ILogger logger)
        {
            this.logger = logger;
            consoleCaller = new TestConsoleCaller();
        }

        public IEnumerable<string> Capabilities => new List<string>
        {
            "TESTING"
        };

        public string InstanceId => "Test Instance";
        public string WorkingDirectory => Environment.CurrentDirectory;

        public bool IsAlive => true;

        public void Init(IRuntime runtime)
        {
            logger.LogInformation("Loading implementation");
            runtime.Container.Get<IPluginManager>().Init();
        }

        public void Reload()
        {
            logger.LogInformation("Reloading implementation");
        }

        public IConsoleCommandCaller GetConsoleCaller()
        {
            return consoleCaller;
        }

        public void Shutdown()
        {
            logger.LogInformation("Shutting down implementation");
        }

        public string Name => "TestImplementation";

        public string ConfigurationName => "TestImplementation";
    }
}