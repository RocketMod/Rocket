using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player;

namespace Rocket.Tests.Mock
{
    public sealed class TestEntity : IPlayerEntity<TestPlayer>, ILivingEntity
    {
        public TestEntity(TestPlayer player)
        {
            Player = player;
        }
        public string EntityTypeName => "";

        public Vector3 Position => new Vector3();
        public Task<bool> TeleportAsync(Vector3 position, float rotation) => throw new NotImplementedException();

        public double MaxHealth => 100;

        public double Health { get => 0; set { } }
        public Task KillAsync() => throw new NotImplementedException();

        public Task KillAsync(IEntity killer) => throw new NotImplementedException();

        public Task KillAsync(IUser killer) => throw new NotImplementedException();
        public TestPlayer Player { get; }
    }


    public sealed class TestPlayer : BasePlayer<TestUser, TestEntity, TestPlayer>
    {
        public TestPlayer(IDependencyContainer container,  IPlayerManager manager) : base(container, manager)
        {
            User = new TestUser(container, this);
        }

        public override bool IsOnline => true;

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

        public override DateTime SessionConnectTime => DateTime.UtcNow;

        public override DateTime? SessionDisconnectTime => DateTime.UtcNow;

        public override TestEntity Entity => new TestEntity(this);
        public override TestUser User { get; }
    }

    public class TestUser : IPlayerUser<TestPlayer>
    {
        public TestUser(IDependencyContainer container, TestPlayer player)
        {
            Container = container;
            Player = player;
        }

        public string Id => "TestPlayerId";
        public string UserName => "TestPlayer";
        public string DisplayName => "TestPlayer";
        public IUserManager UserManager => throw new NotImplementedException();
        public bool IsOnline => true;
        public DateTime? LastSeen => DateTime.UtcNow;

        public List<IIdentity> Identities => throw new NotImplementedException();
        public string UserType => "TestPlayer";

        public IDependencyContainer Container { get; private set; }
        public TestPlayer Player { get; }
        IPlayer IPlayerUser.Player => Player;
    }
}