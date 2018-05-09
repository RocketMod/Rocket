using System;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player;

namespace Rocket.Tests.Mock
{
    public sealed class TestPlayer : BasePlayer, ILivingEntity
    {
        public TestPlayer(IDependencyContainer container, string id, string name) : base(container)
        {
            Id = id;
            Name = name;
        }

        public TestPlayer(IDependencyContainer container) : this(container, "TestPlayerId", "TestPlayer") { }

        public override string Id { get; }
        public override string Name { get; }

        public override IEntity Entity => this;
        public override bool IsOnline => true;

        public override IUser User => new TestUser(this);

        public double MaxHealth
        {
            get => 100;
            set => throw new NotImplementedException();
        }

        public double Health
        {
            get => 0;
            set => throw new NotImplementedException();
        }

        public void Kill()
        {
            throw new NotImplementedException();
        }

        public void Kill(IEntity killer)
        {
            throw new NotImplementedException();
        }

        public void Kill(IUser user)
        {
            throw new NotImplementedException();
        }

        public string EntityTypeName => "Player";
    }

    public class TestUser : IPlayerUser
    {
        private readonly TestPlayer testPlayer;

        public TestUser(TestPlayer testPlayer)
        {
            this.testPlayer = testPlayer;
            SessionConnectTime = DateTime.Now;
            Player = testPlayer;
        }

        public string Id => testPlayer.Id;
        public string Name => testPlayer.Name;
        public IdentityType Type => testPlayer.Type;
        public IUserManager UserManager => throw new NotImplementedException();
        public bool IsOnline => true;
        public DateTime SessionConnectTime { get; }
        public DateTime? SessionDisconnectTime => null;
        public DateTime? LastSeen => DateTime.Now;
        public string UserType => "TestPlayer";
        public IPlayer Player { get; }
    }
}