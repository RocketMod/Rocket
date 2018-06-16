using System;
using Rocket.API.Drawing;
using Rocket.API.User;
using System.Collections.Generic;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Extensions;

namespace Rocket.Core.User
{
    public static class UserExtensions
    {
        public static void SendMessage(this IUser user, string message, params object[] bindings)
        {
            SendMessage(user, message, null, bindings);
        }

        public static void SendMessage(this IUser user, string message, Color? color = null, params object[] bindings)
        {
            user.UserManager.SendMessage(user, message, color, bindings);
        }

        public static TimeSpan GetOnlineTime(this IUser user) 
            => (user.SessionDisconnectTime ?? DateTime.Now) - user.SessionConnectTime;

        public static void SendMessage(this IUserManager manager, IUser sender, IUser receiver, string message,
                                       params object[] arguments)
        {
            manager.SendMessage(sender, receiver, message, null, arguments);
        }

        public static void Broadcast(this IUserManager manager, IUser sender, IEnumerable<IUser> receivers,
                                     string message, params object[] arguments)
        {
            manager.Broadcast(sender, receivers, message, null, arguments);
        }

        public static void Broadcast(this IUserManager manager, IUser sender, string message, params object[] arguments)
        {
            manager.Broadcast(sender, message, null, arguments);
        }

        public static void SendMessage(this IUserManager manager, IUser receiver, string message, Color? color = null,
                                       params object[] arguments)
        {
            manager.SendMessage(null, receiver, message, color, arguments);
        }

        public static void SendMessage(this IUserManager manager, IUser receiver, string message, params object[] arguments)
        {
            manager.SendMessage(null, receiver, message, null, arguments);
        }

        public static IPlayer GetPlayer(this IPlayerUser user)
        {
            return user.GetPrivateProperty<IPlayer>("Player");
        }

        public static bool Kick(this IUser user, IUser kickedBy = null, string reason = null)
        {
            return user.UserManager.Kick(user, kickedBy, reason);
        }

        public static bool Ban(this IUser user, IUser kickedBy = null, string reason = null, TimeSpan? duration = null)
        {
            return user.UserManager.Ban(user, kickedBy, reason, duration);
        }

        public static PermissionResult CheckPermission(this IUser user, string permission)
        {
            var permissionProvider = user.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckPermission(user, permission);
        }

        public static PermissionResult CheckHasAnyPermission(this IUser user, params string[] permissions)
        {
            var permissionProvider = user.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAnyPermission(user, permissions);
        }

        public static PermissionResult CheckHasAllPermissions(this IUser user, params string[] permissions)
        {
            var permissionProvider = user.Container.Resolve<IPermissionProvider>();
            return permissionProvider.CheckHasAllPermissions(user, permissions);
        }
    }
}