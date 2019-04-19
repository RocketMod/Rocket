using System;
using System.Threading.Tasks;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public static class PermissionsExtensions
    {
        /// <summary>
        ///     Checks if the target has all of the given permissions.
        /// </summary>
        /// <param name="permissionChecker">The permission checker.</param>
        /// <param name="target">The target to check.</param>
        /// <param name="permissions">The permissions to check.</param>
        /// <returns>
        ///     <see cref="PermissionResult.Grant" /> if the target explicity has access to all of the given permissions,
        ///     <see cref="PermissionResult.Deny" /> if the target explicitly does not have access to any of the permissions;
        ///     otherwise, <see cref="PermissionResult.Default" />
        /// </returns>
        public static async Task<PermissionResult> CheckHasAllPermissionsAsync(this IPermissionChecker permissionChecker, IPermissionActor target, params string[] permissions)
        {
            PermissionResult result = PermissionResult.Grant;

            foreach (string permission in permissions)
            {
                PermissionResult tmp = await permissionChecker.CheckPermissionAsync(target, permission);
                if (tmp == PermissionResult.Deny)
                    return PermissionResult.Deny;

                if (tmp == PermissionResult.Default)
                    result = PermissionResult.Default;
            }

            return result;
        }

        /// <summary>
        ///     Checks if the target has any of the given permissions.
        /// </summary>
        /// <param name="permissionChecker">The permission checker.</param>
        /// <param name="target">The target to check.</param>
        /// <param name="permissions">The permissions to check.</param>
        /// <returns>
        ///     <see cref="PermissionResult.Grant" /> if the target explicity has access to any of the given permissions,
        ///     <see cref="PermissionResult.Deny" /> if the target explicitly does not have access to any of the permissions;
        ///     otherwise, <see cref="PermissionResult.Default" />
        /// </returns>
        public static async Task<PermissionResult> CheckHasAnyPermissionAsync(this IPermissionChecker permissionChecker, IPermissionActor target, params string[] permissions)
        {
            foreach (string permission in permissions)
            {
                Console.WriteLine("Checking: " + permission);

                PermissionResult result = await permissionChecker.CheckPermissionAsync(target, permission);
                if (result == PermissionResult.Deny)
                {
                    Console.WriteLine("Denied: " + permission);
                    return PermissionResult.Deny;
                }
                if (result == PermissionResult.Grant)
                {
                    Console.WriteLine("Granted: " + permission);
                    return PermissionResult.Grant;
                }

            }

            Console.WriteLine("Default: " + permissions);
            return PermissionResult.Default;
        }
    }
}