using System.Collections.Generic;
using Rocket.API.Player;

namespace Rocket.API.Providers.Implementation.Managers
{
    public interface IPlayerManager
    {
        List<IRocketPlayer> Players { get; }
    }
}