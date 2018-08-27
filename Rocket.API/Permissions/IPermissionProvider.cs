using System;
using System.Collections.Generic;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.User;

namespace Rocket.API.Permissions
{
    /// <summary>
    ///     The Permission Provider is responsible for checking permissions.
    /// </summary>
    public interface IPermissionProvider : IProxyableService
    {
        /// <summary>
        ///     Gets the permissions of the given target.
        /// </summary>
        /// <param name="target">The target whichs permissions to get.</param>
        /// <param name="inherit">Defines if the parent groups permissions should be included.</param>
        /// <returns>A list of all permissions of the target.</returns>
        IEnumerable<string> GetGrantedPermissions(IPermissionEntity target, bool inherit = true);

        /// <summary>
        ///     Gets the denied permissions of the given target.
        /// </summary>
        /// <param name="target">The target whichs denied permissions to get.</param>
        /// <param name="inherit">Defines if the parent groups denied permissions should be included.</param>
        /// <returns>A list of all denied permissions of the target.</returns>
        IEnumerable<string> GetDeniedPermissions(IPermissionEntity target, bool inherit = true);

        /// <summary>
        ///     Defines if the given target is supported by this provider.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns><b>true</b> if the given target is supported; otherwise, <b>false</b>.</returns>
        bool SupportsTarget(IPermissionEntity target);

        /// <summary>
        ///     Check if the target has the given permission.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>
        ///     <see cref="PermissionResult.Grant" /> if the target explicity has the permission,
        ///     <see cref="PermissionResult.Deny" /> if the target explicitly does not have the permission; otherwise,
        ///     <see cref="PermissionResult.Default" />
        /// </returns>
        PermissionResult CheckPermission(IPermissionEntity target, string permission);

        /// <summary>
        ///     Checks if the target has all of the given permissions.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <param name="permissions">The permissions to check.</param>
        /// <returns>
        ///     <see cref="PermissionResult.Grant" /> if the target explicity has access to all of the given permissions,
        ///     <see cref="PermissionResult.Deny" /> if the target explicitly does not have access to any of the permissions;
        ///     otherwise, <see cref="PermissionResult.Default" />
        /// </returns>
        PermissionResult CheckHasAllPermissions(IPermissionEntity target, params string[] permissions);

        /// <summary>
        ///     Checks if the target has any of the given permissions.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <param name="permissions">The permissions to check.</param>
        /// <returns>
        ///     <see cref="PermissionResult.Grant" /> if the target explicity has access to any of the given permissions,
        ///     <see cref="PermissionResult.Deny" /> if the target explicitly does not have access to any of the permissions;
        ///     otherwise, <see cref="PermissionResult.Default" />
        /// </returns>
        PermissionResult CheckHasAnyPermission(IPermissionEntity target, params string[] permissions);

        /// <summary>
        ///     Adds an explicitly granted permission to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The permission to add.</param>
        /// <returns><b>true</b> if the permission was successfully added; otherwise, <b>false</b>.</returns>
        bool AddPermission(IPermissionEntity target, string permission);

        /// <summary>
        ///     Adds an explicitly denied permission to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The denied permission to add.</param>
        /// <returns><b>true</b> if the permission was successfully added; otherwise, <b>false</b>.</returns>
        bool AddDeniedPermission(IPermissionEntity target, string permission);

        /// <summary>
        ///     Removes an explicitly granted permission from the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The permission to remove.</param>
        /// <returns><b>true</b> if the permission was successfully removed; otherwise, <b>false</b>.</returns>
        bool RemovePermission(IPermissionEntity target, string permission);

        /// <summary>
        ///     Removes an explicitly denied permission from the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The permission to remove.</param>
        /// <returns><b>true</b> if the permission was successfully removed; otherwise, <b>false</b>.</returns>
        bool RemoveDeniedPermission(IPermissionEntity target, string permission);

        /// <summary>
        ///     Gets the primary group of the given user.
        /// </summary>
        /// <param name="user">The user wose primary group to get of.</param>
        /// <returns>the primary group if it exists; otherwise, <b>null</b>.</returns>
        [Obsolete("Might be removed")]
        IPermissionGroup GetPrimaryGroup(IUser user);

        /// <summary>
        ///     Gets the primary group with the given ID.
        /// </summary>
        /// <param name="id">The ID of the group.</param>
        /// <returns>the group if it exists; otherwise, <b>null</b>.</returns>
        IPermissionGroup GetGroup(string id);

        /// <summary>
        ///     Gets all inherited groups of the target. If target is a group itself, it will return the parent groups.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>the inherited groups of the target.</returns>
        IEnumerable<IPermissionGroup> GetGroups(IPermissionEntity target);

        /// <summary>
        ///     Gets all registered groups.
        /// </summary>
        /// <returns>all registed groups of this provider.</returns>
        IEnumerable<IPermissionGroup> GetGroups();

        /// <summary>
        ///     Updates a group.
        /// </summary>
        /// <param name="group">The group to update.</param>
        /// <returns><b>true</b> if the group exists and could be updated; otherwise, <b>false</b>.</returns>
        bool UpdateGroup(IPermissionGroup group);

        /// <summary>
        ///     Adds the given group to the user.
        /// </summary>
        /// <param name="target">The target to add the group to.</param>
        /// <param name="group">The group to add.</param>
        /// <returns><b>true</b> if the group was successfully added; otherwise, <b>false</b>.</returns>
        bool AddGroup(IPermissionEntity target, IPermissionGroup group);

        /// <summary>
        ///     Removes the given group from the user.
        /// </summary>
        /// <param name="target">The target to add the group to.</param>
        /// <param name="group">The group to remove.</param>
        /// <returns><b>true</b> if the group was successfully removed; otherwise, <b>false</b>.</returns>
        bool RemoveGroup(IPermissionEntity target, IPermissionGroup group);

        /// <summary>
        ///     Creates a new permission group.
        /// </summary>
        /// <param name="group">The group to create.</param>
        /// <returns><b>true</b> if the group was successfully created; otherwise, <b>false</b>.</returns>
        bool CreateGroup(IPermissionGroup group);

        /// <summary>
        ///     Deletes a permission group.
        /// </summary>
        /// <param name="group">The group to delete.</param>
        /// <returns><b>true</b> if the group was successfully deleted; otherwise, <b>false</b>.</returns>
        bool DeleteGroup(IPermissionGroup group);

        /// <summary>
        ///     Loads the permissions from the given context.
        /// </summary>
        /// <param name="context">the configuration context to load the permissions from.</param>
        void Load(IConfigurationContext context);

        /// <summary>
        ///     Reloads the permissions from the context which was used to initially load them.<br /><br />
        ///     May override not saved changes.
        /// </summary>
        void Reload();

        /// <summary>
        ///     Saves the changes to the permissions.
        /// </summary>
        void Save();
    }
}
