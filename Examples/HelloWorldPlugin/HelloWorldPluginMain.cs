using System.Collections.Generic;
using Rocket.API.Ioc;
using Rocket.API.Logging;
using Rocket.Core.Plugins;

namespace HelloWorldPlugin
{
    public class HelloWorldPluginMain : Plugin
    {
        private readonly ILogger logger;

        public HelloWorldPluginMain(IDependencyContainer container, ILogger logger) : base("HelloWorldPlugin",
            container)
        {
            this.logger = logger;
        }

        protected override void OnLoad()
        {
            logger.LogInformation("Hello world!");
        }

        protected override void OnUnload() { }

        public override IEnumerable<string> Capabilities => new List<string>();
    }
}