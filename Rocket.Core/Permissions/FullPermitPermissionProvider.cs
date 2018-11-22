using System;
using System.Collections.Generic;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.API.User;

namespace Rocket.Core.Permissions
{
    public abstract class FullPermitPermissionProvider<T> : FullPermitPermissionProvider where T : IPermissionEntity
    {
        public override bool SupportsTarget(IPermissionEntity target) => target is T;
    }

    public abstract class FullPermitPermissionProvider : IPermissionProvider
    {
        public IEnumerable<string> GetGrantedPermissions(IPermissionEntity target, bool inherit = true)
        {
            GuardTarget(target);
            return new List<string>();
        }

        public IEnumerable<string> GetDeniedPermissions(IPermissionEntity target, bool inherit = true)
        {
            GuardTarget(target);
            return new List<string>();
        }

        public abstract bool SupportsTarget(IPermissionEntity target);

        public PermissionResult CheckPermission(IPermissionEntity target, string permission)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAllPermissions(IPermissionEntity target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public PermissionResult CheckHasAnyPermission(IPermissionEntity target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public bool AddPermission(IPermissionEntity target, string permission) => false;

        public bool AddDeniedPermission(IPermissionEntity target, string permission) => false;

        public bool RemovePermission(IPermissionEntity target, string permission) => false;

        public bool RemoveDeniedPermission(IPermissionEntity target, string permission) => false;

        public IPermissionGroup GetPrimaryGroup(IUser user) => null;

        public IPermissionGroup GetGroup(string id) => null;

        public IEnumerable<IPermissionGroup> GetGroups(IPermissionEntity target) => new IPermissionGroup[0];

        public IEnumerable<IPermissionGroup> GetGroups() => new IPermissionGroup[0];

        public bool UpdateGroup(IPermissionGroup group) => false;

        public bool AddGroup(IPermissionEntity target, IPermissionGroup group) => false;

        public bool RemoveGroup(IPermissionEntity target, IPermissionGroup group) => false;

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

        private void GuardTarget(IPermissionEntity target)
        {
            if (!SupportsTarget(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }

        public string ServiceName => "ConsolePermissions";
    }
}
