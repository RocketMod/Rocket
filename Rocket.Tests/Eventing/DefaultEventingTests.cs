using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Eventing;

namespace Rocket.Tests.Eventing
{
    [TestClass]
    public class DefaultEventingTests : EventingTestBase
    {
        protected override IEventBus GetEventManager() => Runtime.Container.Resolve<IEventBus>();
    }
}