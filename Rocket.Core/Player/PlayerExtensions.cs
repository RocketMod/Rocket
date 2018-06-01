using Rocket.API.Player;
using Rocket.Core.Extensions;

namespace Rocket.Core.Player
{
    public static class PlayerExtensions
    {
        public static IPlayerUser GetUser(this IPlayer player)
        {
            return player.GetPrivateProperty<IPlayerUser>("User");
        }

        public static IPlayerEntity GetEntity(this IPlayer player)
        {
            return player.GetPrivateProperty<IPlayerEntity>("Entity");
        }
    }
}