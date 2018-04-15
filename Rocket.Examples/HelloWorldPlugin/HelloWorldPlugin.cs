using System.Collections.Generic;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Plugins;

namespace Rocket.Examples.HelloWorldPlugin
{
    public class HelloWorldPlugin : Plugin
    {
        public override object DefaultConfiguration => new
        {
            MyConfigField = "MyFieldValue",
            NestedSection = new
            {
                NestedConfigField = "MyNestedFieldValue"
            }
        };

        private readonly ILogger logger;

        public HelloWorld(IDependencyContainer container, ILogger logger) : base("HelloWorldPlugin",
            container)
        {
            this.logger = logger;
        }

        protected override void OnLoad()
        {
            logger.LogInformation("Hello world!");
        }

        protected override void OnUnload() { }

        public override IEnumerable<string> Capabilities => new List<string> { CapabilityOptions.NoTranslations };
    }
}