using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.Core.Plugins;

namespace Rocket.Tests.Mock
{
    public class TestPlugin : Plugin
    {
        public TestPlugin(IDependencyContainer container) : base(container)
        {
            Logger.LogInformation("Constructing TestPlugin (From plugin)");
        }

        public Task<bool> TestEventing()
        {
            TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();

            Subscribe<TestEvent>((sender, arguments) =>
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

            Subscribe("test", (sender, arguments) =>
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

        protected override void OnActivate(bool isFromReload)
        {
            Logger.LogInformation("Hello World (From plugin)");
        }

        protected override void OnDeactivate()
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