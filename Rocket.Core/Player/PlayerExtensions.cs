using System;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Extensions;

namespace Rocket.Core.Player
{
    public static class PlayerExtensions
    {
        public static IPlayerUser GetUser(this IPlayer player)
        {
            if(player == null)
                throw new ArgumentNullException(nameof(player));

            return player.GetPrivateProperty<IPlayerUser>("User");
        }

        public static IPlayerEntity GetEntity(this IPlayer player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.GetPrivateProperty<IPlayerEntity>("Entity");
        }

        public static bool Kick(this IPlayer player, IUser kickedBy = null, string reason = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.Kick(player.GetUser(), kickedBy, reason);
        }

        public static bool Ban(this IPlayer player, IUser kickedBy = null, string reason = null, TimeSpan? duration = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return player.PlayerManager.Ban(player.GetUser(), kickedBy, reason, duration);
        }

        public static PermissionResult CheckPermission(this IPlayer player, string permission)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckPermission(player, permission);
        }

        public static PermissionResult CheckHasAnyPermission(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAnyPermission(player, permissions);
        }

        public static PermissionResult CheckHasAllPermissions(this IPlayer player, params string[] permissions)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var permissionProvider = player.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAllPermissions(player, permissions);
        }
    }
}