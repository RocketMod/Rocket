using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class ConsolePermissionProvider : IPermissionProvider
    {
        public bool SupportsPermissible(IPermissible permissible) => permissible is IConsoleCommandCaller;

        public PermissionResult CheckPermission(IPermissible target, string permission)
        {
            GuardPermissible(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAllPermissions(IPermissible target, params string[] permissions)
        {
            GuardPermissible(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAnyPermission(IPermissible target, params string[] permissions)
        {
            GuardPermissible(target);
            return PermissionResult.Grant;
        }

        public bool AddPermission(IPermissible target, string permission) => false;

        public bool AddDeniedPermission(IPermissible target, string permission) => false;

        public bool RemovePermission(IPermissible target, string permission) => false;

        public bool RemoveDeniedPermission(IPermissible target, string permission) => false;

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller) => null;

        public IPermissionGroup GetGroup(string id) => null;

        public IEnumerable<IPermissionGroup> GetGroups(IPermissible caller) => new IPermissionGroup[0];

        public IEnumerable<IPermissionGroup> GetGroups() => new IPermissionGroup[0];

        public void UpdateGroup(IPermissionGroup group) { }

        public bool AddGroup(IPermissible target, IPermissionGroup group) => false;

        public bool RemoveGroup(IPermissible caller, IPermissionGroup group) => false;

        public bool CreateGroup(IPermissionGroup group) => false;

        public bool DeleteGroup(IPermissionGroup group) => false;

        public void Load(IConfigurationElement groups, IConfigurationElement players)
        {
            // do nothing
        }

        public void Reload()
        {
            // do nothing
        }

        public void Save()
        {
            // do nothing
        }

        private void GuardPermissible(IPermissible target)
        {
            if (!SupportsPermissible(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }
    }
}