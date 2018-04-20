using System;
using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.API.Player
{
    public interface IPlayerManager
    {
        IEnumerable<IOnlinePlayer> OnlinePlayers { get; }

        bool Kick(IPlayer player, ICommandCaller caller = null, string reason = null);

        bool Ban(IPlayer player, ICommandCaller caller = null, string reason = null, TimeSpan? timeSpan = null);

        IOnlinePlayer GetOnlinePlayer(string nameOrId);
        IOnlinePlayer GetOnlinePlayerByName(string displayName);
        IOnlinePlayer GetOnlinePlayerById(string id);

        bool TryGetOnlinePlayer(string nameOrId, out IOnlinePlayer output);
        bool TryGetOnlinePlayerById(string uniqueID, out IOnlinePlayer output);
        bool TryGetOnlinePlayerByName(string displayName, out IOnlinePlayer output);

        IPlayer GetPlayer(string id); //guaranteed to not return null
    }
}