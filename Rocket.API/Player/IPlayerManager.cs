using System;
using System.Collections.Generic;

namespace Rocket.API.Player
{
    public interface IPlayerManager
    {
        bool Kick(IPlayer player, string reason);

        bool Ban(IPlayer player, string reason, TimeSpan? timeSpan = null);

        IEnumerable<IPlayer> Players { get; }

        IPlayer GetPlayer(string uniqueID);

        IPlayer GetPlayerByName(string displayName);

        bool TryGetPlayer(string uniqueID, out IPlayer output);

        bool TryGetPlayerByName(string displayName, out IPlayer output);
    }
}
