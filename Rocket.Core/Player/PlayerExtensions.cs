using System;
using System.Threading.Tasks;
using Rocket.API.Entities;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Extensions;
using Rocket.Core.Permissions;

namespace Rocket.Core.Player
{
    public static class EntityExtensions
    {
        public static Task<bool> KickAsync(this IPlayer player, IUser kickedBy = null, string reason = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.KickAsync(player.User, kickedBy, reason);
        }

        public static Task<bool> BanAsync(this IPlayer player, IUser kickedBy = null, string reason = null, TimeSpan? duration = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.BanAsync(player.User, kickedBy, reason, duration);
        }

        public static Task<PermissionResult> CheckPermissionAsync(this IPlayer player, string permission)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionChecker = player.Container.Resolve<IPermissionChecker>();
            return permissionChecker.CheckPermissionAsync(player.User, permission);
        }

        public static Task<PermissionResult> CheckHasAnyPermissionAsync(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionChecker>();
            return permissionProvider.CheckHasAnyPermissionAsync(player.User, permissions);
        }

        public static Task<PermissionResult> CheckHasAllPermissionsAsync(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionChecker>();
            return permissionProvider.CheckHasAllPermissionsAsync(player.User, permissions);
        }
    }
}