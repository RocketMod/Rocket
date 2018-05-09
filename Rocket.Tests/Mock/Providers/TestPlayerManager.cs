using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Tests.Mock.Providers
{
    public class TestPlayerManager : IPlayerManager
    {
        public TestPlayerManager(IDependencyContainer container)
        {
            Container = container;
        }

        public IDependencyContainer Container { get; }

        public IEnumerable<IPlayer> OnlinePlayers => new List<IPlayer> {new TestPlayer(Container)};

        public IEnumerable<IUser> Users => OnlinePlayers.Select(c => c.User);
        public bool Kick(IUser user, IUser kickedBy = null, string reason = null) => false;

        public bool Ban(IUserInfo user, IUser bannedBy = null, string reason = null, TimeSpan? timeSpan = null)
            => false;

        public bool Unban(IUserInfo user, IUser unbannedBy = null)
            => false;

        public void SendMessage(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void Broadcast(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null,
                              params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public IPlayer GetOnlinePlayer(string nameOrId)
        {
            return OnlinePlayers.FirstOrDefault(c => c.Id.Equals(nameOrId, StringComparison.OrdinalIgnoreCase)
                || c.Name.Equals(nameOrId, StringComparison.OrdinalIgnoreCase));
        }

        public IPlayer GetOnlinePlayerByName(string name) => GetOnlinePlayer(name);

        public IPlayer GetOnlinePlayerById(string id) => GetOnlinePlayer(id);

        public bool TryGetOnlinePlayer(string nameOrId, out IPlayer output)
            => throw new NotImplementedException();

        public bool TryGetOnlinePlayerById(string uniqueID, out IPlayer output)
            => throw new NotImplementedException();

        public bool TryGetOnlinePlayerByName(string displayName, out IPlayer output)
            => throw new NotImplementedException();

        public IPlayer GetPlayer(string id) => GetOnlinePlayer(id);
    }
}