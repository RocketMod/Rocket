using System;
using System.Collections.Generic;
using System.Numerics;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player;

namespace Rocket.Tests.Mock
{
    public sealed class TestEntity : IPlayerEntity, ILivingEntity
    {
        public string EntityTypeName => "";

        public Vector3 Position => new Vector3();

        public double MaxHealth => 100;

        public double Health { get => 0; set { } }
    
        public void Kill()
        {
            throw new NotImplementedException();
        }

        public void Kill(IEntity killer)
        {
            throw new NotImplementedException();
        }

        public void Kill(IUser killer)
        {
            throw new NotImplementedException();
        }

        public bool Teleport(Vector3 position)
        {
            throw new NotImplementedException();
        }
    }


    public sealed class TestPlayer : BasePlayer<TestUser, TestEntity>
    {
        public override string ToString()
        {
            return User.DisplayName;
        }
        public TestPlayer(IDependencyContainer container,  IPlayerManager manager) : base(container, manager)
        {
            User = new TestUser(container);
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

        public override DateTime SessionConnectTime => DateTime.Now;

        public override DateTime? SessionDisconnectTime => DateTime.Now;

        public override TestEntity Entity => new TestEntity();
        public override TestUser User { get; }
    }

    public class TestUser : IUser
    {
        public TestUser(IDependencyContainer container)
        {
            Container = container;
        }

        public string Id => "TestPlayerId";
        public string UserName => "TestPlayer";
        public string DisplayName => "TestPlayer";
        public UserType Type => UserType.Player;
        public IUserManager UserManager => throw new NotImplementedException();
        public bool IsOnline => true;
        public DateTime? LastSeen => DateTime.Now;

        public List<IIdentity> Identities => throw new NotImplementedException();

        public IDependencyContainer Container { get; private set; }
    }
}