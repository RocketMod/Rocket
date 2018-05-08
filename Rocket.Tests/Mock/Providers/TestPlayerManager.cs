using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Rocket.API.Commands;
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
        public bool Kick(IUser player, IUser caller = null, string reason = null) => false;

        public bool Ban(IUser player, IUser caller = null, string reason = null, TimeSpan? timeSpan = null)
            => false;

        public bool Unban(IUser player, IUser caller = null) 
            => false;

        public void SendMessage(IUser sender, IUser receiver, string message, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IUser sender, IEnumerable<IUser> receivers, string message, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IUser sender, string message, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments)
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