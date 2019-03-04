using System.Drawing;
using Rocket.API.Player;
using Rocket.API.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;

namespace Rocket.Tests.Mock.Providers
{
    public class TestPlayerManager : IPlayerManager
    {
        private readonly TestPlayer testPlayer;

        public TestPlayerManager(IDependencyContainer container)
        {
            testPlayer = new TestPlayer(container, this);
        }

        public Task<bool> KickAsync(IPlayer user, IUser kickedBy = null, string reason = null) => throw new NotImplementedException();

        public Task<IEnumerable<IPlayer>> GetPlayersAsync() => throw new NotImplementedException();

        public async Task<IPlayer> GetPlayerAsync(string nameOrId) => testPlayer;

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
        public Task<bool> KickAsync(IUser user, IUser kickedBy = null, string reason = null) => throw new NotImplementedException();

        public Task SendMessageAsync(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments) => throw new NotImplementedException();

        public Task BroadcastAsync(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null,
                                   params object[] arguments) => throw new NotImplementedException();

        public Task BroadcastAsync(IUser sender, string message, Color? color = null, params object[] arguments) => throw new NotImplementedException();

        public Task<IUser> GetUserAsync(string id) => throw new NotImplementedException();
    }
}