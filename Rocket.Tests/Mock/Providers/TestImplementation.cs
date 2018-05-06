using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Core.Logging;

namespace Rocket.Tests.Mock.Providers
{
    public class TestImplementation : IImplementation
    {
        private readonly ILogger logger;

        public TestImplementation(ILogger logger)
        {
            this.logger = logger;
            ConsoleCommandCaller = new TestConsoleCaller();
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
            runtime.Container.Resolve<IPluginManager>().Init();
        }

        public void Reload()
        {
            logger.LogInformation("Reloading implementation");
        }

        public IConsoleCommandCaller ConsoleCommandCaller { get; }

        public void Shutdown()
        {
            logger.LogInformation("Shutting down implementation");
        }

        public string Name => "TestImplementation";

        public string ConfigurationName => "TestImplementation";
    }
}