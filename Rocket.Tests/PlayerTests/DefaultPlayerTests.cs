using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Player;

namespace Rocket.Tests.PlayerTests
{
    [TestClass]
    public class DefaultPlayerTests : PlayerTestBase
    {
        protected override IPlayerManager GetPlayerManager()
        {
            return Runtime.Container.Get<IPlayerManager>();
        }
    }
}