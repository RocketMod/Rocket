using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Core.Logging;
using Rocket.Core.User;

namespace Rocket.Tests.Mock.Providers
{
    public class TestHost : IHost
    {
        private readonly ILogger logger;

        public TestHost(IDependencyContainer container, ILogger logger)
        {
            this.logger = logger;
            Console = new StdConsole(container);
        }

        public IEnumerable<string> Capabilities => new List<string>
        {
            "TESTING"
        };
        public Version HostVersion => new Version(FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).FileVersion);
        public Version GameVersion => HostVersion;
        public string ServerName => "Rocket Test Host";
        public ushort ServerPort => 0;
        public string WorkingDirectory => Environment.CurrentDirectory;

        public bool IsAlive => true;

        public string GameName => "Rocket.Test";

        public async Task InitAsync(IRuntime runtime)
        {
            logger.LogInformation("Loading host");
            await runtime.Container.Resolve<IPluginLoader>().InitAsync();
        }

        public async Task ReloadAsync()
        {
            logger.LogInformation("Reloading host");
        }

        public IConsole Console { get; }

        public async Task ShutdownAsync()
        {
            logger.LogInformation("Shutting down host");
        }

        public string Name => "TestHost";

        public string ConfigurationName => "TestHost";
    }
}