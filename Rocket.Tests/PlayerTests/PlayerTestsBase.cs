using System.Runtime.Remoting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Player;

namespace Rocket.Tests.PlayerTests
{
    [TestCategory("Players")]
    public abstract class PlayerTestsBase : RocketTestBase
    {
        public TestPlayer TestPlayer { get; private set; }

        [TestInitialize]
        public override void Bootstrap()
        {
            base.Bootstrap();
            TestPlayer = new TestPlayer();
        }

        [TestMethod]
        public void TestFormatting()
        {
            Assert.AreEqual(TestPlayer.Name, string.Format("{0}", TestPlayer));
            Assert.AreEqual(TestPlayer.Name, string.Format("{0:Name}", TestPlayer));
            Assert.AreEqual(TestPlayer.Id, string.Format("{0:Id}", TestPlayer));
        }

        protected abstract IPlayerManager GetPlayerManager();
    }
}