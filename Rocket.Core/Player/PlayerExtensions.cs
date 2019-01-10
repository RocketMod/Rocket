using System;
using System.Threading.Tasks;
using Rocket.API.Entities;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Extensions;

namespace Rocket.Core.Player
{
    public static class EntityExtensions
    {
        public static Task<bool> KickAsync(this IPlayer player, IUser kickedBy = null, string reason = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.KickAsync(player.GetUser(), kickedBy, reason);
        }

        public static Task<bool> BanAsync(this IPlayer player, IUser kickedBy = null, string reason = null, TimeSpan? duration = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.BanAsync(player.GetUser(), kickedBy, reason, duration);
        }

        public static Task<PermissionResult> CheckPermissionAsync(this IPlayer player, string permission)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckPermissionAsync(player.GetUser(), permission);
        }

        public static Task<PermissionResult> CheckHasAnyPermissionAsync(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAnyPermissionAsync(player.GetUser(), permissions);
        }

        public static Task<PermissionResult> CheckHasAllPermissionsAsync(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAllPermissionsAsync(player.GetUser(), permissions);
        }

        public static IUser GetUser(this IPlayer player)
        {
            return player.GetPrivateProperty<IUser>("User");
        }


        public static IEntity GetEntity(this IPlayer player)
        {
            return player.GetPrivateProperty<IEntity>("Entity");
        }
    }
}