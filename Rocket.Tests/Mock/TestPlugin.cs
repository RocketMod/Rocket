using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.Core.Eventing;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace Rocket.Tests.Mock
{
    public class TestPlugin : Plugin
    {
        public TestPlugin(IDependencyContainer container) : base("TestPlugin", container)
        {
            Logger.LogInformation("Constructing TestPlugin (From plugin)");
        }

        public Task<bool> TestEventing()
        {
            TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();

            Subscribe<TestEvent>(async (sender, arguments) =>
            {
                Assert.Equals(sender, this);
                promise.SetResult(arguments.Value);
            });

            Emit(new TestEvent
            {
                Value = true
            });

            return promise.Task;
        }

        public Task<bool> TestEventingWithName()
        {
            TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();

            Subscribe("test", async (sender, arguments) =>
            {
                Assert.Equals(sender, this);
                promise.SetResult(((TestEvent) arguments).Value);
            });

            Emit(new TestEvent
            {
                Value = true
            });

            return promise.Task;
        }

        protected override async Task OnActivate(bool isFromReload)
        {
            Logger.LogInformation("Hello World (From plugin)");
        }

        protected override async Task OnDeactivate()
        {
            Logger.LogInformation("Bye World (From plugin)");
        }

        private class TestEvent : Event
        {
            public TestEvent() : base(EventExecutionTargetContext.Sync) { }

            public bool Value { get; set; }
        }
    }
}