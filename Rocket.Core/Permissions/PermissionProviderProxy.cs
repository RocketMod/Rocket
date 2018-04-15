using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.ServiceProxies;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Permissions
{
    public class PermissionProviderProxy : IPermissionProvider, IServiceProxy<IPermissionProvider>
    {
        private readonly IDependencyContainer container;
        public IEnumerable<IPermissionProvider> ProxiedProviders
        {
            get
            {
                var providers = container.GetAll<IPermissionProvider>()
                                         .Where(c => c != this)
                                         .ToList();

                ServicePriorityComparer.Sort(providers, true);
                return providers;
            }
        }
        public PermissionProviderProxy(IDependencyContainer container)
        {
            this.container = container;
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return ProxiedProviders.Any(c => c.SupportsCaller(caller));
        }

        public bool SupportsGroup(IPermissionGroup group)
        {
            return ProxiedProviders.Any(c => c.SupportsGroup(group));
        }

        public PermissionResult HasPermission(IPermissionGroup @group, string permission)
        {
            foreach (var provider in ProxiedProviders.Where(c => c.SupportsGroup(group)))
            {
                PermissionResult result = provider.HasPermission(group, permission);
                if(result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public PermissionResult HasPermission(ICommandCaller caller, string permission)
        {
            foreach (var provider in ProxiedProviders.Where(c => c.SupportsCaller(caller)))
            {
                PermissionResult result = provider.HasPermission(caller, permission);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public PermissionResult HasAllPermissions(IPermissionGroup @group, params string[] permissions)
        {
            foreach (var provider in ProxiedProviders.Where(c => c.SupportsGroup(group)))
            {
                PermissionResult result = provider.HasAllPermissions(@group, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public PermissionResult HasAllPermissions(ICommandCaller caller, params string[] permissions)
        {
            foreach (var provider in ProxiedProviders.Where(c => c.SupportsCaller(caller)))
            {
                PermissionResult result = provider.HasAllPermissions(caller, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public PermissionResult HasAnyPermissions(IPermissionGroup @group, params string[] permissions)
        {
            foreach (var provider in ProxiedProviders.Where(c => c.SupportsGroup(group)))
            {
                PermissionResult result = provider.HasAnyPermissions(@group, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public PermissionResult HasAnyPermissions(ICommandCaller caller, params string[] permissions)
        {
            foreach (var provider in ProxiedProviders.Where(c => c.SupportsCaller(caller)))
            {
                PermissionResult result = provider.HasAnyPermissions(caller, permissions);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        public bool AddPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException("Adding permissions from proxy is not supported.");
        }

        public bool AddDeniedPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException("Adding inverted permissions from proxy is not supported.");
        }

        public bool AddPermission(ICommandCaller caller, string permission)
        {
            throw new System.NotSupportedException("Adding permissions from proxy is not supported.");
        }

        public bool AddDeniedPermission(ICommandCaller caller, string permission)
        {
            throw new System.NotSupportedException("Adding inverted permissions from proxy is not supported.");
        }

        public bool RemovePermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException("Removing permissions from proxy is not supported.");
        }

        public bool RemoveDeniedPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotSupportedException("Removing inverted permissions from proxy is not supported.");
        }

        public bool RemovePermission(ICommandCaller caller, string permission)
        {
            throw new System.NotSupportedException("Removing permissions from proxy is not supported.");
        }

        public bool RemoveDeniedPermission(ICommandCaller @group, string permission)
        {
            throw new System.NotSupportedException("Removing denied permissions from proxy is not supported.");
        }

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            throw new System.NotSupportedException("Getting primary group from proxy is not supported.");
        }

        public IPermissionGroup GetGroup(string id)
        {
            throw new System.NotSupportedException("Getting a group via id from proxy is not supported.");
        }

        public IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller)
        {
            return ProxiedProviders.SelectMany(c => c.GetGroups(caller));
        }

        public IEnumerable<IPermissionGroup> GetGroups()
        {
            return ProxiedProviders.SelectMany(c => c.GetGroups());
        }

        public void UpdateGroup(IPermissionGroup @group)
        {
            throw new System.NotSupportedException("Updating groups from proxy is not supported.");
        }

        public void AddGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            throw new System.NotSupportedException("Adding groups from proxy is not supported.");
        }

        public bool RemoveGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            throw new System.NotSupportedException("Removing groups from proxy is not supported.");
        }

        public void CreateGroup(IPermissionGroup @group)
        {
            throw new System.NotSupportedException("Creating groups from proxy is not supported.");
        }

        public void DeleteGroup(IPermissionGroup @group)
        {
            throw new System.NotSupportedException("Deleting groups from proxy is not supported.");
        }

        public void Load(IConfigurationElement groups, IConfigurationElement players)
        {
            throw new System.NotSupportedException("Loading from proxy is not supported.");
        }

        public void Reload()
        {
            ProxiedProviders.ForEach(c => c.Reload());
        }

        public void Save()
        {
            ProxiedProviders.ForEach(c => c.Save());
        }
    }
}