using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Permissions
{
    public class ProxyPermissionProvider : ServiceProxy<IPermissionProvider>, IPermissionProvider
    {
        public ProxyPermissionProvider(IDependencyContainer container) : base(container) { }

        public bool SupportsTarget(IIdentifiable target)
        {
            return ProxiedServices.Any(c => c.SupportsTarget(target));
        }

        public PermissionResult CheckPermission(IIdentifiable target, string permission)
        {
            GuardTarget(target);

            foreach (IPermissionProvider provider in ProxiedServices.Where(c => c.SupportsTarget(target)))
            {
                PermissionResult result = provider.CheckPermission(target, permission);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public PermissionResult CheckHasAllPermissions(IIdentifiable target, params string[] permissions)
        {
            GuardTarget(target);

            foreach (IPermissionProvider provider in ProxiedServices.Where(c => c.SupportsTarget(target)))
            {
                PermissionResult result = provider.CheckHasAllPermissions(target, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public PermissionResult CheckHasAnyPermission(IIdentifiable target, params string[] permissions)
        {
            GuardTarget(target);

            foreach (IPermissionProvider provider in ProxiedServices.Where(c => c.SupportsTarget(target)))
            {
                PermissionResult result = provider.CheckHasAnyPermission(target, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public bool AddPermission(IIdentifiable target, string permission)
            => throw new NotSupportedException("Adding permissions from proxy is not supported.");

        public bool AddDeniedPermission(IIdentifiable target, string permission)
            => throw new NotSupportedException("Adding inverted permissions from proxy is not supported.");

        public bool RemovePermission(IIdentifiable target, string permission)
            => throw new NotSupportedException("Removing permissions from proxy is not supported.");

        public bool RemoveDeniedPermission(IIdentifiable target, string permission)
            => throw new NotSupportedException("Removing inverted permissions from proxy is not supported.");

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            IPermissionGroup group;
            foreach (IPermissionProvider service in ProxiedServices.Where(c => c.SupportsTarget(caller)))
                if ((group = service.GetPrimaryGroup(caller)) != null)
                    return group;

            return null;
        }

        public IPermissionGroup GetGroup(string id)
        {
            return GetGroups().FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IPermissionGroup> GetGroups(IIdentifiable target)
        {
            return ProxiedServices.Where(c => c.SupportsTarget(target))
                                  .SelectMany(c => c.GetGroups(target));
        }

        public IEnumerable<IPermissionGroup> GetGroups()
        {
            return ProxiedServices.SelectMany(c => c.GetGroups());
        }

        public void UpdateGroup(IPermissionGroup group)
        {
            throw new NotSupportedException("Updating groups from proxy is not supported.");
        }

        public bool AddGroup(IIdentifiable target, IPermissionGroup group)
            => throw new NotSupportedException("Adding groups from proxy is not supported.");

        public bool RemoveGroup(IIdentifiable target, IPermissionGroup group)
            => throw new NotSupportedException("Removing groups from proxy is not supported.");

        public bool CreateGroup(IPermissionGroup group)
            => throw new NotSupportedException("Creating groups from proxy is not supported.");

        public bool DeleteGroup(IPermissionGroup group)
            => throw new NotSupportedException("Deleting groups from proxy is not supported.");

        public void Load(IConfigurationContext context)
        {
            ProxiedServices.ForEach(c => c.Load(context));
        }

        public void Reload()
        {
            ProxiedServices.ForEach(c => c.Reload());
        }

        public void Save()
        {
            ProxiedServices.ForEach(c => c.Save());
        }

        private void GuardTarget(IIdentifiable target)
        {
            if (!SupportsTarget(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }
    }
}