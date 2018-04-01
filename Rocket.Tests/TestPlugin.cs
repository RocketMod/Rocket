using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Scheduler;
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

        class TestEvent : Event
        {
            public bool Value { get; set; }

            public TestEvent() : base(EventExecutionTargetContext.Sync)
            {

            }
        }

        public Task<bool> TestEventing()
        {
            var promise = new TaskCompletionSource<bool>();

            Subscribe<TestEvent>((sender, arguments) =>
            {
                Assert.Equals(sender, this);
                promise.SetResult(arguments.Value);
            });

            Emit(new TestEvent { Value = true});

            return promise.Task;
        }

        public Task<bool> TestEventing_WithName()
        {
            var promise = new TaskCompletionSource<bool>();

            Subscribe("test", (sender, arguments) =>
            {
                Assert.Equals(sender, this);
                promise.SetResult(((TestEvent)arguments).Value);
            });

            Emit(new TestEvent { Value = true });

            return promise.Task;
        }

        protected override void OnLoad()
        {
            Logger.Info("Hello World (From plugin)");
        }

        protected override void OnUnload()
        {
            Logger.Info("Bye World (From plugin)");
        }

    }
}
