using System;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Core.Player
{
    public static class PlayerExtensions
    {
        public static bool Kick(this IPlayer player, IUser kickedBy = null, string reason = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.Kick(player, kickedBy, reason);
        }

        public static bool Ban(this IPlayer player, IUser kickedBy = null, string reason = null, TimeSpan? duration = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.Ban(player.User, kickedBy, reason, duration);
        }

        public static PermissionResult CheckPermission(this IPlayer player, string permission)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckPermission(player.User, permission);
        }

        public static PermissionResult CheckHasAnyPermission(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAnyPermission(player.User, permissions);
        }

        public static PermissionResult CheckHasAllPermissions(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAllPermissions(player.User, permissions);
        }
    }
}