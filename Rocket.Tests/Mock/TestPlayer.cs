using System;
using Rocket.API.DependencyInjection;
using Rocket.Core.Player;

namespace Rocket.Tests.Mock
{
    public sealed class TestPlayer : BaseOnlinePlayer
    {
        public TestPlayer(IDependencyContainer container, string id, string name) : base(container)
        {
            Id = id;
            Name = name;
        }

        public TestPlayer(IDependencyContainer container) : this(container, "TestPlayerId", "TestPlayer")
        {

        }

        public override string Id { get; }
        public override string Name { get; }
        public override Type CallerType => typeof(TestPlayer);

        public override void SendMessage(string message, ConsoleColor? color)
        {
            Console.WriteLine("[TestPlayer.SendMessage] " + message);
        }

        public override DateTime SessionConnectTime { get; } = DateTime.Now;
        public override DateTime? SessionDisconnectTime => null;
        public override TimeSpan SessionOnlineTime => DateTime.Now - SessionConnectTime;

        public override bool IsOnline => true;
        public override DateTime? LastSeen => DateTime.Now;
    }
}