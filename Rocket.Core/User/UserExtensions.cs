using System;
using System.Drawing;
using Rocket.API.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Extensions;
using Rocket.Core.Permissions;

namespace Rocket.Core.User
{
    public static class UserExtensions
    {
        public static async Task SendMessageAsync(this IUser user, string message, params object[] bindings)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await SendMessageAsync(user, message, null, bindings);
        }

        public static async Task SendMessageAsync(this IUser user, string message, Color? color = null, params object[] bindings)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await user.UserManager.SendMessageAsync(user, message, color, bindings);
        }

        public static async Task SendMessageAsync(this IUserManager manager, IUser sender, IUser receiver, string message,
                                       params object[] arguments)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            await manager.SendMessageAsync(sender, receiver, message, null, arguments);
        }

        public static async Task BroadcastAsync(this IUserManager manager, IUser sender, IEnumerable<IUser> receivers,
                                     string message, params object[] arguments)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            await manager.BroadcastAsync(sender, receivers, message, null, arguments);
        }

        public static async Task BroadcastAsync(this IUserManager manager, IUser sender, string message, params object[] arguments)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            await manager.BroadcastAsync(sender, message, null, arguments);
        }

        public static async Task SendMessageAsync(this IUserManager manager, IUser receiver, string message, Color? color = null,
                                       params object[] arguments)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            await manager.SendMessageAsync(null, receiver, message, color, arguments);
        }

        public static async Task SendMessageAsync(this IUserManager manager, IUser receiver, string message, params object[] arguments)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            await manager.SendMessageAsync(null, receiver, message, null, arguments);
        }

        public static async Task<bool> BanAsync(this IUser user, IUser kickedBy = null, string reason = null, TimeSpan? duration = null)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await user.UserManager.BanAsync(user, kickedBy, reason, duration);
        }

        public static async Task<PermissionResult> CheckPermissionAsync(this IUser user, string permission)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var permissionChecker = user.Container.Resolve<IPermissionChecker>();
            return await permissionChecker.CheckPermissionAsync(user, permission);
        }

        public static async Task<PermissionResult> CheckHasAnyPermissionAsync(this IUser user, params string[] permissions)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var permissionChecker = user.Container.Resolve<IPermissionChecker>();
            return await permissionChecker.CheckHasAnyPermissionAsync(user, permissions);
        }

        public static async Task<PermissionResult> CheckHasAllPermissionsAsync(this IUser user, params string[] permissions)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var permissionChecker = user.Container.Resolve<IPermissionChecker>();
            return await permissionChecker.CheckHasAllPermissionsAsync(user, permissions);
        }
    }
}