using System.Collections.Generic;
using Rocket.API.Player;

namespace Rocket.API.Implementation.Managers
{
    public delegate void PlayerConnected(IRocketPlayer player);
    public delegate void PlayerDisconnected(IRocketPlayer player);

    public interface IPlayerManager
    {
        List<IRocketPlayer> Players { get; }
        event PlayerConnected OnPlayerConnected;
        event PlayerDisconnected OnPlayerDisconnected;
    }
}