using System.Collections.Generic;
using Rocket.API.Player;

namespace Rocket.API.Implementation.Managers
{
    public interface IPlayerManager
    {
        List<IRocketPlayer> Players { get; }
    }
}