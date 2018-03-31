using Rocket.API;
using Rocket.API.Logging;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;

namespace Rocket.Tests
{
    public class TestPlugin : IPlugin
    {
        public IEnumerable<string> Capabilities => new List<string>() { "TESTING" };

        public IEventEmitter Sender { get; }
        public string Name => "Test Plugin";

        public IDependencyContainer Container;

        public bool IsAsync { get; }

        public State State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ILogger logger;

        public bool Loaded = false;

        public TestPlugin(ILogger logger)
        {
            this.logger = logger;
            logger.Info("Constructing TestPlugin (From plugin)");
        }

        public void Load()
        {
            Loaded = true;
            logger.Info("Hello World (From plugin)");
        }

        public void Unload()
        {
            logger.Info("Bye World (From plugin)");
        }
    }
}
