using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;

namespace Rocket.Tests.Mock.Providers
{
    public class TestPlayerManager : IPlayerManager
    {
        public TestPlayerManager(IDependencyContainer container)
        {
            Container = container;
        }

        public IDependencyContainer Container { get; }

        public IEnumerable<IOnlinePlayer> OnlinePlayers => new List<IOnlinePlayer> {new TestPlayer(Container)};

        public bool Kick(IOnlinePlayer player, ICommandCaller caller = null, string reason = null) => false;

        public bool Ban(IPlayer player, ICommandCaller caller = null, string reason = null, TimeSpan? timeSpan = null)
            => false;

        public bool Unban(IPlayer player, ICommandCaller caller = null)
            => false;

        public IOnlinePlayer GetOnlinePlayer(string nameOrId)
        {
            return OnlinePlayers.FirstOrDefault(c => c.Id.Equals(nameOrId, StringComparison.OrdinalIgnoreCase)
                || c.Name.Equals(nameOrId, StringComparison.OrdinalIgnoreCase));
        }

        public IOnlinePlayer GetOnlinePlayerByName(string name) => GetOnlinePlayer(name);

        public IOnlinePlayer GetOnlinePlayerById(string id) => GetOnlinePlayer(id);

        public bool TryGetOnlinePlayer(string nameOrId, out IOnlinePlayer output)
            => throw new NotImplementedException();

        public bool TryGetOnlinePlayerById(string uniqueID, out IOnlinePlayer output)
            => throw new NotImplementedException();

        public bool TryGetOnlinePlayerByName(string displayName, out IOnlinePlayer output)
            => throw new NotImplementedException();

        public IPlayer GetPlayer(string id) => GetOnlinePlayer(id);
    }
}