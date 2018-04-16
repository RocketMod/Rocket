using System;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Plugins;

namespace Rocket.Tests
{
    public sealed class TestPlayer : BasePlayer
    {
        public TestPlayer(IDependencyContainer container, string id, string name) : base(container)
        {
            Id = id;
            Name = name;
        }

        public TestPlayer(IDependencyContainer container) : this(container, "TestPlayerId", "TestPlayer")
        {

        }

        public override string Id { get; protected set; }
        public override string Name { get; protected set; }
        public override Type CallerType => typeof(TestPlayer);

        public override void SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override double Health { get => 0; set {} }
        public override double MaxHealth { get => 100; set { } }
    }
}