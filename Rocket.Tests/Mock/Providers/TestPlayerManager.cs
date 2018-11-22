using Rocket.API.Drawing;
using Rocket.API.Player;
using Rocket.API.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rocket.Tests.Mock.Providers
{
    public class TestPlayerManager : IPlayerManager
    {
        public Task<bool> KickAsync(IPlayer user, IUser kickedBy = null, string reason = null) => throw new NotImplementedException();

        public Task<IEnumerable<IPlayer>> GetPlayersAsync() => throw new NotImplementedException();

        public Task<IPlayer> GetPlayerAsync(string nameOrId) => throw new NotImplementedException();

        public Task<IPlayer> GetPlayerByNameAsync(string name) => throw new NotImplementedException();

        public Task<IPlayer> GetPlayerByIdAsync(string id) => throw new NotImplementedException();

        public bool TryGetOnlinePlayer(string nameOrId, out IPlayer output)
            => throw new NotImplementedException();

        public bool TryGetOnlinePlayerById(string uniqueID, out IPlayer output)
            => throw new NotImplementedException();

        public bool TryGetOnlinePlayerByName(string displayName, out IPlayer output)
            => throw new NotImplementedException();


        public string ServiceName => "TestPlayers";
        public Task<IIdentity> GetIdentity(string id) => throw new NotImplementedException();

        public Task<bool> BanAsync(IUser user, IUser bannedBy = null, string reason = null, TimeSpan? duration = null) => throw new NotImplementedException();

        public Task<bool> UnbanAsync(IUser user, IUser unbannedBy = null) => throw new NotImplementedException();

        public Task SendMessageAsync(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments) => throw new NotImplementedException();

        public Task BroadcastAsync(IUser sender, IEnumerable<IPlayer> receivers, string message, Color? color = null,
                                   params object[] arguments) => throw new NotImplementedException();

        public Task BroadcastAsync(IUser sender, string message, Color? color = null, params object[] arguments) => throw new NotImplementedException();

        public Task<IUser> GetUserAsync(string id) => throw new NotImplementedException();
    }
}