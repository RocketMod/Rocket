using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Serialization;
using Rocket.API.Permissions;
using System;

namespace Rocket.Core.Providers.Permissions
{
    public class PermissionsProviderProxy : ProxyBase<IPermissionsProvider>, IPermissionsProvider
    {

        public void Save()
        {
            InvokeAll(provider => provider.Save());
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(IPlayer player)
        {
            var list = new List<RocketPermissionsGroup>();
            foreach (var prov in R.Providers.GetProviders<IPermissionsProvider>())
            {
                list.AddRange(prov.GetPlayerGroups(player));
            }
            return list.AsReadOnly();
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(string playerId)
        {
            var list = new List<RocketPermissionsGroup>();
            foreach (var prov in R.Providers.GetProviders<IPermissionsProvider>())
            {
                list.AddRange(prov.GetPlayerGroups(playerId));
            }
            return list.AsReadOnly();
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            foreach (var prov in R.Providers.GetProviders<IPermissionsProvider>())
            {
                var group = prov.GetGroup(groupId);
                if(group != null)
                    return group;
            }

            return null;
        }

        public bool AddGroup(RocketPermissionsGroup @group)
        {
            throw new System.NotSupportedException("AddGroup not supported on Proxy provider");
        }

        public bool SaveGroup(RocketPermissionsGroup @group)
        {
            bool wasSuccess = false;
            foreach (var prov in R.Providers.GetProviders<IPermissionsProvider>())
            {
                var g = prov.GetGroup(group.Id);
                if (g != null)
                {
                    if (prov.SaveGroup(@group))
                        wasSuccess = true;
                }
            }

            return wasSuccess;
        }

        public bool DeleteGroup(RocketPermissionsGroup @group)
        {
            bool wasSuccess = false;
            foreach (var prov in R.Providers.GetProviders<IPermissionsProvider>())
            {
                var g = prov.GetGroup(group.Id);
                if (g != null)
                {
                    if (prov.DeleteGroup(@group))
                        wasSuccess = true;
                }
            }

            return wasSuccess;
        }

        public PermissionResult CheckPermission(IPlayer player, string permission)
        {
            PermissionResult result = new PermissionResult(PermissionResultType.DEFAULT, PermissionPriority.LOWEST);
            foreach (var prov in R.Providers.GetProviders<IPermissionsProvider>())
            {
                var provResult = prov.CheckPermission(player, permission);
                if (provResult.Priority > result.Priority)
                {
                    result = provResult;
                    continue;
                }

                if (provResult.Result > result.Result)
                {
                    result = provResult;
                }
            }

            return result;
        }

        public PermissionResult CheckPermission(string id, string permission)
        {
            PermissionResult result = new PermissionResult(PermissionResultType.DEFAULT, PermissionPriority.LOWEST);
            foreach (var prov in R.Providers.GetProviders<IPermissionsProvider>())
            {
                var provResult = prov.CheckPermission(id, permission);

                if (provResult.Priority < result.Priority)
                {
                    continue;
                }

                if (provResult.Priority > result.Priority)
                {
                    result = provResult;
                    continue;
                }

                if (provResult.Result > result.Result)
                {
                    result = provResult;
                }
            }

            return result;
        }

        protected override void OnLoad(ProviderManager providerManager)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }
    }
}