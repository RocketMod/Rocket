using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface IChatProvider
    {
        void Say(IPlayer player, string message, Color? color = null);
        void Say(string message, Color? color = null);
    }
}
