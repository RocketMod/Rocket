using System;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Plugins;

namespace Rocket.Tests
{
    public sealed class TestPlayer : BasePlayer
    {
        public TestPlayer(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public TestPlayer() : this("TestPlayerId", "TestPlayer")
        {
            
        }

        public override string Id { get; protected set; }
        public override string Name { get; protected set; }
        public override Type CallerType => typeof(TestPlayer);

        public override void SendMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}