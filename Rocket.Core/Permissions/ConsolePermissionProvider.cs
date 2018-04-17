using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class ConsolePermissionProvider : IPermissionProvider
    {
        public bool SupportsPermissible(IPermissible permissible)
        {
            return permissible is IConsoleCommandCaller;
        }
        
        public PermissionResult HasPermission(IPermissible target, string permission)
        {
            GuardPermissible(target);
            return PermissionResult.Grant;
        }

        public PermissionResult HasAllPermissions(IPermissible target, params string[] permissions)
        {
            GuardPermissible(target);
            return PermissionResult.Grant;
        }

        public PermissionResult HasAnyPermissions(IPermissible target, params string[] permissions)
        {
            GuardPermissible(target);
            return PermissionResult.Grant;
        }

        public bool AddPermission(IPermissible target, string permission)
        {
            return false;
        }

        public bool AddDeniedPermission(IPermissible target, string permission)
        {
            return false;
        }

        public bool RemovePermission(IPermissible target, string permission)
        {
            return false;
        }

        public bool RemoveDeniedPermission(IPermissible target, string permission)
        {
            return false;
        }

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            return null;
        }

        public IPermissionGroup GetGroup(string id)
        {
            return null;
        }

        public IEnumerable<IPermissionGroup> GetGroups(IPermissible caller)
        {
            return new IPermissionGroup[0];
        }

        public IEnumerable<IPermissionGroup> GetGroups()
        {
            return new IPermissionGroup[0];
        }

        public void UpdateGroup(IPermissionGroup @group)
        {
            
        }

        public bool AddGroup(IPermissible target, IPermissionGroup @group)
        {
            return false;
        }

        public bool RemoveGroup(IPermissible caller, IPermissionGroup @group)
        {
            return false;
        }

        public bool CreateGroup(IPermissionGroup @group)
        {
            return false;
        }

        public bool DeleteGroup(IPermissionGroup @group)
        {
            return false;
        }

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