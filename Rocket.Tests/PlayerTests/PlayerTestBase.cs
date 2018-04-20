using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Remoting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Player;
using Rocket.Core.Extensions;
using Rocket.Tests.Mock;

namespace Rocket.Tests.PlayerTests
{
    [TestCategory("Players")]
    public abstract class PlayerTestBase : RocketTestBase
    {
        public TestPlayer TestPlayer { get; private set; }

        [TestInitialize]
        public override void Bootstrap()
        {
            base.Bootstrap();
            TestPlayer = new TestPlayer(Runtime.Container);
        }

        [TestMethod]
        public void TestFormatting()
        {
            Assert.AreEqual(TestPlayer.Name, $"{TestPlayer}");
            Assert.AreEqual(TestPlayer.Name, $"{TestPlayer:Name}");
            Assert.AreEqual(TestPlayer.Id, $"{TestPlayer:Id}");

            Assert.AreEqual("0.00", string.Format(CultureInfo.InvariantCulture, "{0:Health:0.00}", TestPlayer));
            Assert.AreEqual("0", string.Format(CultureInfo.InvariantCulture, "{0:Health}", TestPlayer));
            Assert.AreEqual("0", string.Format(CultureInfo.InvariantCulture, "{0:Health:0}", TestPlayer));
            Assert.AreEqual("0", $"{TestPlayer:Health}");
            Assert.AreEqual("0", $"{TestPlayer:Health:0}");

            Assert.AreEqual("100.00", string.Format(CultureInfo.InvariantCulture, "{0:MaxHealth:0.00}", TestPlayer));
            Assert.AreEqual("100", string.Format(CultureInfo.InvariantCulture, "{0:MaxHealth}", TestPlayer));
            Assert.AreEqual("100", string.Format(CultureInfo.InvariantCulture, "{0:MaxHealth:0}", TestPlayer));
            Assert.AreEqual("100", $"{TestPlayer:MaxHealth}");
            Assert.AreEqual("100", $"{TestPlayer:MaxHealth:0}");

            Assert.ThrowsException<FormatException>(() => string.Format(CultureInfo.InvariantCulture, "{0:invalid_format}", TestPlayer));
        }

        [TestMethod]
        public void TestConverterts()
        {
            TypeConverter playerConverter = TypeConverterExtensions.GetConverter(typeof(IPlayer));
            TypeConverter onlinePlayerConverter = TypeConverterExtensions.GetConverter(typeof(IOnlinePlayer));

            Assert.AreEqual(TestPlayer.Id, ((IPlayer)playerConverter.ConvertFromWithContext(Runtime.Container, TestPlayer.Id)).Id);
            Assert.AreEqual(TestPlayer.Id, ((IOnlinePlayer)onlinePlayerConverter.ConvertFromWithContext(Runtime.Container, TestPlayer.Id)).Id);
        }

        protected abstract IPlayerManager GetPlayerManager();
    }
}