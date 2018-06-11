using System;
using System.Numerics;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player;

namespace Rocket.Tests.Mock
{
    public sealed class TestPlayer : BasePlayer<TestPlayer, TestUser, TestPlayer>, 
        IPlayerEntity<TestPlayer>, 
        ILivingEntity
    {
        public TestPlayer(IDependencyContainer container, string id, string name) : base(container)
        {
            Id = id;
            Name = name;
        }

        public TestPlayer(IDependencyContainer container) : this(container, "TestPlayerId", "TestPlayer") { }

        public override string Id { get; }
        public override string Name { get; }

        public override TestPlayer Entity => this;
        public override bool IsOnline => true;

        public override TestUser User => new TestUser(this);

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
        public TestPlayer Player => this;
        public bool Teleport(Vector3 position)
        {
            return false;
        }
    }

    public class TestUser : IPlayerUser<TestPlayer>
    {
        private readonly TestPlayer testPlayer;

        public TestUser(TestPlayer testPlayer)
        {
            this.testPlayer = testPlayer;
            SessionConnectTime = DateTime.Now;
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
        public IDependencyContainer Container => Player.Container;
        public TestPlayer Player => testPlayer;
    }
}