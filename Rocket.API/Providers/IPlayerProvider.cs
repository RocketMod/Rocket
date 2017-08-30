using System.Collections.Generic;
using Rocket.API.Player;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface IPlayerProvider
    {
        List<IPlayer> Players { get; }
        void Kick(IPlayer caller, string reason);
    }
}