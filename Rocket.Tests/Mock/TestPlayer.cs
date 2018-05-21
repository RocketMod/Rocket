using System;
using System.Numerics;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player;

namespace Rocket.Tests.Mock
{
    public sealed class TestPlayer : BasePlayer, IPlayerEntity, ILivingEntity
    {
        public TestPlayer(IDependencyContainer container, string id, string name) : base(container)
        {
            Id = id;
            Name = name;
        }

        public TestPlayer(IDependencyContainer container) : this(container, "TestPlayerId", "TestPlayer") { }

        public override string Id { get; }
        public override string Name { get; }

        public override IPlayerEntity Entity => this;
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
        public Vector3 Position => Vector3.Zero;
        public IPlayer Player => this;
        public bool Teleport(Vector3 position)
        {
            return false;
        }
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
        public string IdentityType => testPlayer.IdentityType;
        public IUserManager UserManager => throw new NotImplementedException();
        public bool IsOnline => true;
        public DateTime SessionConnectTime { get; }
        public DateTime? SessionDisconnectTime => null;
        public DateTime? LastSeen => DateTime.Now;
        public string UserType => "TestPlayer";
        public IPlayer Player { get; }
    }
}