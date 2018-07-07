using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Permissions
{
    public abstract class FullPermitPermissionProvider<T> : FullPermitPermissionProvider where T : IIdentity
    {
        public override bool SupportsTarget(IIdentity target) => target is T;
    }

    public abstract class FullPermitPermissionProvider : IPermissionProvider
    {
        public IEnumerable<string> GetGrantedPermissions(IIdentity target, bool inherit = true)
        {
            GuardTarget(target);
            return new List<string>();
        }

        public IEnumerable<string> GetDeniedPermissions(IIdentity target, bool inherit = true)
        {
            GuardTarget(target);
            return new List<string>();
        }

        public abstract bool SupportsTarget(IIdentity target);

        public PermissionResult CheckPermission(IIdentity target, string permission)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAllPermissions(IIdentity target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAnyPermission(IIdentity target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public bool AddPermission(IIdentity target, string permission) => false;

        public bool AddDeniedPermission(IIdentity target, string permission) => false;

        public bool RemovePermission(IIdentity target, string permission) => false;

        public bool RemoveDeniedPermission(IIdentity target, string permission) => false;

        public IPermissionGroup GetPrimaryGroup(IUser user) => null;

        public IPermissionGroup GetGroup(string id) => null;

        public IEnumerable<IPermissionGroup> GetGroups(IIdentity target) => new IPermissionGroup[0];

        public IEnumerable<IPermissionGroup> GetGroups() => new IPermissionGroup[0];

        public bool UpdateGroup(IPermissionGroup group) => false;

        public bool AddGroup(IIdentity target, IPermissionGroup group) => false;

        public bool RemoveGroup(IIdentity target, IPermissionGroup group) => false;

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

        private void GuardTarget(IIdentity target)
        {
            if (!SupportsTarget(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }

        public string ServiceName => "ConsolePermissions";
    }
}
