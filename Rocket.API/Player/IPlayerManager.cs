using System;
using System.Collections.Generic;

namespace Rocket.API.Player
{
    public interface IPlayerManager
    {
        IEnumerable<IPlayer> Players { get; }
        bool Kick(IPlayer player, IPlayer kicker = null, string reason = null);

        bool Ban(IPlayer player, IPlayer banner = null, string reason = null, TimeSpan? timeSpan = null);

        IPlayer GetPlayer(string uniqueID);

        IPlayer GetPlayerByName(string displayName);

        bool TryGetPlayer(string uniqueID, out IPlayer output);

        bool TryGetPlayerByName(string displayName, out IPlayer output);
    }
}