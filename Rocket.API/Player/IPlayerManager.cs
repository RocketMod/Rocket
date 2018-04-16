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

        IPlayer GetPlayer(string nameOrId);
        IPlayer GetPlayerById(string id);
        IPlayer GetPlayerByName(string displayName);

        bool TryGetPlayer(string uniqueID, out IPlayer output);

        bool TryGetPlayerByName(string displayName, out IPlayer output);
    }
}