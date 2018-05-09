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
            IEventManager manager = GetEventManager();
            manager.Subscribe<TestEvent>(GetListener(), (sender, @event) => @event.ValueChanged = true);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync);
            EmitTestEvent(manager, e);

            Assert.AreEqual(true, e.ValueChanged, "The subscription callback did not get called");
        }

        [TestMethod]
        public virtual void TestSyncEventingWithName()
        {
            IEventManager manager = GetEventManager();
            manager.Subscribe(GetListener(), "test", (sender, @event) => ((TestEvent) @event).ValueChanged = true);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync);
            EmitTestEvent(manager, e);

            Assert.AreEqual(true, e.ValueChanged, "The subscription callback did not get called");
        }

        [TestMethod]
        public virtual void TestCancellationWithoutIgnore()
        {
            IEventManager manager = GetEventManager();
            manager.Subscribe<TestEvent>(GetListener(), (sender, @event) => @event.ValueChanged = true);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync) {IsCancelled = true};
            EmitTestEvent(manager, e);

            Assert.AreEqual(false, e.ValueChanged,
                "The subscription callback was called on a cancelled event when it shouldn't be");
        }

        [TestMethod]
        public virtual void TestCancellationWithIgnore()
        {
            IEventManager manager = GetEventManager();
            manager.Subscribe<TestEvent>(GetListener(), CancelIgnoreEventHandler);

            TestEvent e = new TestEvent(EventExecutionTargetContext.Sync) {IsCancelled = true};
            EmitTestEvent(manager, e);

            Assert.AreEqual(true, e.ValueChanged,
                "The subscription callback was not called on a cancelled event but it has IgnoreCancelled set to true");
        }

        [EventHandler(IgnoreCancelled = true)]
        private void CancelIgnoreEventHandler(IEventEmitter sender, IEvent @event)
        {
            ((TestEvent) @event).ValueChanged = true;
        }

        private void EmitTestEvent(IEventManager manager, TestEvent @event)
        {
            bool finished = false;
            manager.Emit(GetEmitter(), @event, e => finished = true);

            while (!finished)
                ;
        }

        protected virtual IEventEmitter GetEmitter() => Runtime.Container.Resolve<IImplementation>();

        protected virtual ILifecycleObject GetListener() => Runtime.Container.Resolve<IRuntime>();

        protected abstract IEventManager GetEventManager();
    }
}