using Rocket.API.Player;

namespace Rocket.Core.Entities
{
    public static class EntityExtensions
    {
        private static IPlayerEntity<IPlayer> Extend(this IPlayerEntity entity)
        {
            return (IPlayerEntity<IPlayer>) entity;
        }

        public static IPlayer GetPlayer(this IPlayerEntity entity)
        {
            return entity.Extend().Player;
        }
    }
}