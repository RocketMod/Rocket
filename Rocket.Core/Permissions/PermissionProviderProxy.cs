using Microsoft.Practices.ObjectBuilder2;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.ServiceProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Core.Permissions
{
    public class PermissionProviderProxy : ServiceProxy<IPermissionProvider>, IPermissionProvider
    {
        public PermissionProviderProxy(IDependencyContainer container) : base(container) { }

        public async Task<IEnumerable<string>> GetGrantedPermissionsAsync(IPermissionEntity target, bool inherit = true)
        {
            return ProxiedServices
                   .Where(c => c.SupportsTarget(target))
                   .SelectMany(c => c.GetGrantedPermissionsAsync(target, inherit).GetAwaiter().GetResult());
        }

        public async Task<IEnumerable<string>> GetDeniedPermissionsAsync(IPermissionEntity target, bool inherit = true)
        {
            return ProxiedServices
                   .Where(c => c.SupportsTarget(target))
                   .SelectMany(c => c.GetDeniedPermissionsAsync(target, inherit).GetAwaiter().GetResult());
        }

        public bool SupportsTarget(IPermissionEntity target)
        {
            return ProxiedServices.Any(c => c.SupportsTarget(target));
        }

        public async Task<PermissionResult> CheckPermissionAsync(IPermissionEntity target, string permission)
        {
            GuardTarget(target);

            foreach (IPermissionProvider provider in ProxiedServices.Where(c => c.SupportsTarget(target)))
            {
                PermissionResult result = await provider.CheckPermissionAsync(target, permission);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public async Task<PermissionResult> CheckHasAllPermissionsAsync(IPermissionEntity target, params string[] permissions)
        {
            GuardTarget(target);

            foreach (IPermissionProvider provider in ProxiedServices.Where(c => c.SupportsTarget(target)))
            {
                PermissionResult result = await provider.CheckHasAllPermissionsAsync(target, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public async Task<PermissionResult> CheckHasAnyPermissionAsync(IPermissionEntity target, params string[] permissions)
        {
            GuardTarget(target);

            foreach (IPermissionProvider provider in ProxiedServices.Where(c => c.SupportsTarget(target)))
            {
                PermissionResult result = await provider.CheckHasAnyPermissionAsync(target, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public Task<bool> AddPermissionAsync(IPermissionEntity target, string permission)
            => throw new NotSupportedException("Adding permissions from proxy is not supported.");

        public Task<bool> AddDeniedPermissionAsync(IPermissionEntity target, string permission)
            => throw new NotSupportedException("Adding inverted permissions from proxy is not supported.");

        public Task<bool> RemovePermissionAsync(IPermissionEntity target, string permission)
            => throw new NotSupportedException("Removing permissions from proxy is not supported.");

        public Task<bool> RemoveDeniedPermissionAsync(IPermissionEntity target, string permission)
            => throw new NotSupportedException("Removing inverted permissions from proxy is not supported.");

        public async Task<IPermissionGroup> GetPrimaryGroupAsync(IPermissionEntity user)
        {
            IPermissionGroup group;
            foreach (IPermissionProvider service in ProxiedServices.Where(c => c.SupportsTarget(user)))
                if ((group = await service.GetPrimaryGroupAsync(user)) != null)
                    return group;

            return null;
        }

        public async Task<IPermissionGroup> GetGroupAsync(string id)
        {
            return (await GetGroupsAsync()).FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<IPermissionGroup>> GetGroupsAsync(IPermissionEntity target)
        {
            return ProxiedServices.Where(c => c.SupportsTarget(target))
                                  .SelectMany(c => c.GetGroupsAsync(target).GetAwaiter().GetResult());
        }

        public async Task<IEnumerable<IPermissionGroup>> GetGroupsAsync()
        {
            return ProxiedServices.SelectMany(c => c.GetGroupsAsync().GetAwaiter().GetResult());
        }

        public Task<bool> UpdateGroupAsync(IPermissionGroup group)
            => throw new NotSupportedException("Updating groups from proxy is not supported.");

        public Task<bool> AddGroupAsync(IPermissionEntity target, IPermissionGroup group)
            => throw new NotSupportedException("Adding groups from proxy is not supported.");

        public Task<bool> RemoveGroupAsync(IPermissionEntity target, IPermissionGroup group)
            => throw new NotSupportedException("Removing groups from proxy is not supported.");

        public Task<bool> CreateGroupAsync(IPermissionGroup group)
            => throw new NotSupportedException("Creating groups from proxy is not supported.");

        public Task<bool> DeleteGroupAsync(IPermissionGroup group)
            => throw new NotSupportedException("Deleting groups from proxy is not supported.");

        public async Task LoadAsync(IConfigurationContext context)
        {
            foreach (var service in ProxiedServices)
                await service.LoadAsync(context);
        }

        public async Task ReloadAsync()
        {
            foreach (var service in ProxiedServices)
                await service.ReloadAsync();
        }

        public async Task SaveAsync()
        {
            foreach (var service in ProxiedServices)
                await service.SaveAsync();
        }

        private void GuardTarget(IPermissionEntity target)
        {
            if (!SupportsTarget(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }

        public string ServiceName => "ProxyPermissions";
    }
}