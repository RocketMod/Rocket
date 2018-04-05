using System.Collections.Generic;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Plugins;

namespace HelloWorldPlugin
{
    public class HelloWorldPluginMain : PluginBase
    {
        private readonly ILogger _logger;

        public HelloWorldPluginMain(IDependencyContainer container, ILogger logger) : base("HelloWorldPlugin", container)
        {
            _logger = logger;
        }

        protected override void OnLoad()
        {
            _logger.Info("Hello world!");
        }

        protected override void OnUnload()
        {

        }

        public override IEnumerable<string> Capabilities => new List<string>();
    }
}
