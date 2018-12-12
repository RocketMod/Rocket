using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API;
using Rocket.API.Eventing;
using Rocket.Core.Eventing;

namespace Rocket.Tests.Eventing
{
    [TestCategory("Eventing")]
    public abstract class EventingTestBase : RocketTestBase
    {
        [TestMethod]
        public virtual void TestSyncEventingWithType()
        {
            IEventBus bus = GetEventBus();
            bus.Subscribe<TestEvent>(GetListener(), async (sender, @event) => @event.ValueChanged = true);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync);
            EmitTestEvent(bus, e);

            Assert.AreEqual(true, e.ValueChanged, "The subscription callback did not get called");
        }

        [TestMethod]
        public virtual void TestSyncEventingWithName()
        {
            IEventBus bus = GetEventBus();
            bus.Subscribe(GetListener(), "test", async (sender, @event) => ((TestEvent) @event).ValueChanged = true);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync);
            EmitTestEvent(bus, e);

            Assert.AreEqual(true, e.ValueChanged, "The subscription callback did not get called");
        }

        [TestMethod]
        public virtual void TestCancellationWithoutIgnore()
        {
            IEventBus bus = GetEventBus();
            bus.Subscribe<TestEvent>(GetListener(), async (sender, @event) => @event.ValueChanged = true);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync) {IsCancelled = true};
            EmitTestEvent(bus, e);

            Assert.AreEqual(false, e.ValueChanged,
                "The subscription callback was called on a cancelled event when it shouldn't be");
        }

        [TestMethod]
        public virtual void TestCancellationWithIgnore()
        {
            IEventBus bus = GetEventBus();
            bus.Subscribe<TestEvent>(GetListener(), CancelIgnoreEventHandler);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync) {IsCancelled = true};
            EmitTestEvent(bus, e);

            Assert.AreEqual(true, e.ValueChanged,
                "The subscription callback was not called on a cancelled event but it has IgnoreCancelled set to true");
        }

        [EventHandler(IgnoreCancelled = true)]
        private async Task CancelIgnoreEventHandler(IEventEmitter sender, IEvent @event)
        {
            ((TestEvent) @event).ValueChanged = true;
        }

        private void EmitTestEvent(IEventBus bus, TestEvent @event)
        {
            bool finished = false;
            bus.Emit(GetEmitter(), @event, async e => finished = true);

            while (!finished)
                ;
        }

        protected virtual IEventEmitter GetEmitter() => Runtime.Container.Resolve<IHost>();

        protected virtual ILifecycleObject GetListener() => Runtime.Container.Resolve<IRuntime>();

        protected abstract IEventBus GetEventBus();
    }
}