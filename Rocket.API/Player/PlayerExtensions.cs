namespace Rocket.API.Player
{
    public static class PlayerExtensions
    {
        public static IPlayer<IPlayerEntity<IPlayer>, IPlayerUser<IPlayer>, IPlayer> Extend(this IPlayer player)
        {
            return ((IPlayer<IPlayerEntity<IPlayer>, IPlayerUser<IPlayer>, IPlayer>) player);
        }
    }
}