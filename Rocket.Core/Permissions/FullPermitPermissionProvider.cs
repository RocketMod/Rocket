using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<string>> GetGrantedPermissionsAsync(IPermissionEntity target, bool inherit = true)
        {
            GuardTarget(target);
            return new List<string>();
        }

        public async Task<IEnumerable<string>> GetDeniedPermissionsAsync(IPermissionEntity target, bool inherit = true)
        {
            GuardTarget(target);
            return new List<string>();
        }

        public abstract bool SupportsTarget(IPermissionEntity target);

        public async Task<PermissionResult> CheckPermissionAsync(IPermissionEntity target, string permission)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public async Task<PermissionResult> CheckHasAllPermissionsAsync(IPermissionEntity target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public async Task<PermissionResult> CheckHasAnyPermissionAsync(IPermissionEntity target, params string[] permissions)
        {
            GuardTarget(target);
            return PermissionResult.Grant;
        }

        public async Task<bool> AddPermissionAsync(IPermissionEntity target, string permission) => false;

        public async Task<bool> AddDeniedPermissionAsync(IPermissionEntity target, string permission) => false;

        public async Task<bool> RemovePermissionAsync(IPermissionEntity target, string permission) => false;

        public async Task<bool> RemoveDeniedPermissionAsync(IPermissionEntity target, string permission) => false;

        public async Task<IPermissionGroup> GetPrimaryGroupAsync(IPermissionEntity user) => null;

        public async Task<IPermissionGroup> GetGroupAsync(string id) => null;

        public async Task<IEnumerable<IPermissionGroup>> GetGroupsAsync(IPermissionEntity target) => new IPermissionGroup[0];

        public async Task<IEnumerable<IPermissionGroup>> GetGroupsAsync() => new IPermissionGroup[0];

        public async Task<bool> UpdateGroupAsync(IPermissionGroup group) => false;

        public async Task<bool> AddGroupAsync(IPermissionEntity target, IPermissionGroup group) => false;

        public async Task<bool> RemoveGroupAsync(IPermissionEntity target, IPermissionGroup group) => false;

        public async Task<bool> CreateGroupAsync(IPermissionGroup group) => false;

        public async Task<bool> DeleteGroupAsync(IPermissionGroup group) => false;

        public async Task LoadAsync(IConfigurationContext context)
        {
            // do nothing
        }

        public async Task ReloadAsync()
        {
            // do nothing
        }

        public async Task SaveAsync()
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
