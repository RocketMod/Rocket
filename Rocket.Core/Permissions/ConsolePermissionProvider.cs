using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class ConsolePermissionProvider : IPermissionProvider
    {
        public bool SupportsTarget(IIdentifiable target) => target is IConsoleCommandCaller;

        public PermissionResult CheckPermission(IIdentifiable target, string permission)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAllPermissions(IIdentifiable target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAnyPermission(IIdentifiable target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public bool AddPermission(IIdentifiable target, string permission) => false;

        public bool AddDeniedPermission(IIdentifiable target, string permission) => false;

        public bool RemovePermission(IIdentifiable target, string permission) => false;

        public bool RemoveDeniedPermission(IIdentifiable target, string permission) => false;

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller) => null;

        public IPermissionGroup GetGroup(string id) => null;

        public IEnumerable<IPermissionGroup> GetGroups(IIdentifiable caller) => new IPermissionGroup[0];

        public IEnumerable<IPermissionGroup> GetGroups() => new IPermissionGroup[0];

        public bool UpdateGroup(IPermissionGroup group) => false;

        public bool AddGroup(IIdentifiable target, IPermissionGroup group) => false;

        public bool RemoveGroup(IIdentifiable caller, IPermissionGroup group) => false;

        public bool CreateGroup(IPermissionGroup group) => false;

        public bool DeleteGroup(IPermissionGroup group) => false;

        public void Load(IConfigurationContext context)
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

        private void GuardTarget(IIdentifiable target)
        {
            if (!SupportsTarget(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }
    }
}