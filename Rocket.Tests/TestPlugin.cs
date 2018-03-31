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
using Rocket.Core.Plugins;
using Rocket.Core.Eventing;

namespace Rocket.Tests
{
    public class TestPlugin : PluginBase
    {
        public override IEnumerable<string> Capabilities => new List<string>() { "TESTING" };

        public override string Name => "Test Plugin";

        public TestPlugin(IDependencyContainer container) : base(container)
        {
            Logger.Info("Constructing TestPlugin (From plugin)");

        }

        public Task<bool> TestEventing()
        {
            var promise = new TaskCompletionSource<bool>();

            Subscribe("TestEvent", (IEventArguments arguments) =>
            {
                promise.SetResult((bool)arguments.Values[0]);
            });

            Emit("TestEvent", new EventArguments(true));

            return promise.Task;
        }


        public override void Load()
        {
            Logger.Info("Hello World (From plugin)");
        }

        public override void Unload()
        {
            Logger.Info("Bye World (From plugin)");
        }
    }
}
