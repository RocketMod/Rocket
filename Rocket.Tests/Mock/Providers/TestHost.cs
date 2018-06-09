using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Core.Logging;

namespace Rocket.Tests.Mock.Providers
{
    public class TestHost : IHost
    {
        private readonly ILogger logger;

        public TestHost(IDependencyContainer container, ILogger logger)
        {
            this.logger = logger;
            Console = new TestConsole(container);
        }

        public IEnumerable<string> Capabilities => new List<string>
        {
            "TESTING"
        };

        public string InstanceId => "Test Instance";
        public string ServerName => "Rocket Test Host";
        public ushort ServerPort => 0;
        public string WorkingDirectory => Environment.CurrentDirectory;

        public bool IsAlive => true;

        public void Init(IRuntime runtime)
        {
            logger.LogInformation("Loading host");
            runtime.Container.Resolve<IPluginManager>().Init();
        }

        public void Reload()
        {
            logger.LogInformation("Reloading host");
        }

        public IConsole Console { get; }

        public void Shutdown()
        {
            logger.LogInformation("Shutting down host");
        }

        public string Name => "TestHost";

        public string ConfigurationName => "TestHost";
    }
}