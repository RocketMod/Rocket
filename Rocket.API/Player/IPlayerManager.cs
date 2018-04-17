using System;
using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.API.Player
{
    public interface IPlayerManager
    {
        IEnumerable<IPlayer> Players { get; }
        bool Kick(IPlayer player, ICommandCaller kicker = null, string reason = null);

        bool Ban(IPlayer player, ICommandCaller banner = null, string reason = null, TimeSpan? timeSpan = null);

        IOnlinePlayer GetOnlinePlayer(string nameOrId);
        IOnlinePlayer GetOnlinePlayerByName(string displayName);
        IOnlinePlayer GetOnlinePlayerById(string id);

        bool TryGetOnlinePlayer(string stringIdOrName, out IOnlinePlayer output);
        bool TryGetOnlinePlayerById(string uniqueID, out IOnlinePlayer output);
        bool TryGetOnlinePlayerByName(string displayName, out IOnlinePlayer output);

        IPlayer GetPlayer(string id); //guaranteed to return non null
    }
}