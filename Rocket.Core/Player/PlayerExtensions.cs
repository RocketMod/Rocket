using Rocket.API.Player;

namespace Rocket.Core.Player
{
    public static class PlayerExtensions
    {
        private static IPlayer<IPlayerEntity<IPlayer>, IPlayerUser<IPlayer>, IPlayer> Extend(this IPlayer player)
        {
            return ((IPlayer<IPlayerEntity<IPlayer>, IPlayerUser<IPlayer>, IPlayer>) player);
        }

        public static IPlayerUser<IPlayer> GetUser(this IPlayer player)
        {
            return player.GetUser();
        }

        public static IPlayerEntity<IPlayer> GetEntity(this IPlayer player)
        {
            return player.Extend().Entity;
        }
    }
}