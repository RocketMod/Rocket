using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Providers.Implementation.Managers
{
    public interface IChatManager
    {
        void Say(IRocketPlayer player, string message, Color? color = null);
        void Say(string message, Color? color = null);
    }
}
