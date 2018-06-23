using System;
using Rocket.API.Player;
using Rocket.Core.Extensions;

namespace Rocket.Core.Entities
{
    public static class EntityExtensions
    {
        public static IPlayer GetPlayer(this IPlayerEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity.GetPrivateProperty<IPlayer>("Player");
        }
    }
}