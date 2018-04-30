using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Player;

namespace Rocket.Tests.Player
{
    [TestClass]
    public class DefaultPlayerTests : PlayerTestBase
    {
        protected override IPlayerManager GetPlayerManager() => Runtime.Container.Resolve<IPlayerManager>();
    }
}