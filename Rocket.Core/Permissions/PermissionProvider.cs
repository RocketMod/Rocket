using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class PermissionProvider : IPermissionProvider
    {
        public bool HasPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotImplementedException();
        }

        public bool HasPermission(ICommandCaller caller, string permission)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAllPermissions(IPermissionGroup @group, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAllPermissions(ICommandCaller caller, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAnyPermissions(IPermissionGroup @group, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAnyPermissions(ICommandCaller caller, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller)
        {
            throw new System.NotImplementedException();
        }
    }
}