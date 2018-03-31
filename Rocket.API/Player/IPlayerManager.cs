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
    }
}
