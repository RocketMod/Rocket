using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class ConsolePermissionProvider : IPermissionProvider
    {
        public bool SupportsCaller(ICommandCaller caller)
        {
            return caller is IConsoleCommandCaller;
        }

        public bool SupportsGroup(IPermissionGroup @group)
        {
            return false;
        }

        public PermissionResult HasPermission(IPermissionGroup @group, string permission)
        {
            return PermissionResult.Grant;
        }

        public PermissionResult HasPermission(ICommandCaller caller, string permission)
        {
            return PermissionResult.Grant;
        }

        public PermissionResult HasAllPermissions(IPermissionGroup @group, params string[] permissions)
        {
            return PermissionResult.Grant;
        }

        public PermissionResult HasAllPermissions(ICommandCaller caller, params string[] permissions)
        {
            return PermissionResult.Grant;
        }

        public PermissionResult HasAnyPermissions(IPermissionGroup @group, params string[] permissions)
        {
            return PermissionResult.Grant;
        }

        public PermissionResult HasAnyPermissions(ICommandCaller caller, params string[] permissions)
        {
            return PermissionResult.Grant;
        }

        public bool AddPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException();
        }

        public bool AddDeniedPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException();
        }

        public bool AddPermission(ICommandCaller caller, string permission)
        {
            throw new System.NotSupportedException("Adding permissions to console is not supported.");
        }

        public bool AddDeniedPermission(ICommandCaller caller, string permission)
        {
            throw new System.NotSupportedException("Adding denied permissions to console is not supported.");
        }

        public bool RemovePermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException();
        }

        public bool RemoveDeniedPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException();
        }

        public bool RemovePermission(ICommandCaller caller, string permission)
        {
            throw new System.NotSupportedException("Removing permissions from console is not supported.");
        }

        public bool RemoveDeniedPermission(ICommandCaller @group, string permission)
        {
            throw new System.NotSupportedException("Removing denied permissions from console is not supported.");
        }

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            throw new System.NotSupportedException("Getting primary group of console is not supported.");
        }

        public IPermissionGroup GetGroup(string id)
        {
            throw new System.NotSupportedException();
        }

        public IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller)
        {
            throw new System.NotSupportedException("Getting groups of console is not supported.");
        }

        public IEnumerable<IPermissionGroup> GetGroups()
        {
            throw new System.NotSupportedException();
        }

        public void UpdateGroup(IPermissionGroup @group)
        {
            throw new System.NotSupportedException();
        }

        public void AddGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            throw new System.NotSupportedException("Adding groups to console is not supported.");
        }

        public bool RemoveGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            throw new System.NotSupportedException("Removing groups from console is not supported.");
        }

        public void CreateGroup(IPermissionGroup @group)
        {
            throw new System.NotSupportedException();
        }

        public void DeleteGroup(IPermissionGroup @group)
        {
            throw new System.NotSupportedException();
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
    }
}