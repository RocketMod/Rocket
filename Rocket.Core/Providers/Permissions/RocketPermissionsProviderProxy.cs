using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Permissions;
using Rocket.API.Serialization;

namespace Rocket.Core.Providers.Permissions
{
    [RocketProviderProxy]
    public class RocketPermissionsProviderProxy : IRocketPermissionsDataProvider
    {
        public void Unload(bool isReload = false)
        {
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
            {
                prov.Unload(isReload);
            }
        }

        public void Load(bool isReload = false)
        {
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
            {
                prov.Load(isReload);
            }
        }

        public void Save()
        {
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
            {
                prov.Save();
            }
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(IRocketPlayer player)
        {
            var list = new List<RocketPermissionsGroup>();
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
            {
                list.AddRange(prov.GetPlayerGroups(player));
            }
            return list.AsReadOnly();
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(string playerId)
        {
            var list = new List<RocketPermissionsGroup>();
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
            {
                list.AddRange(prov.GetPlayerGroups(playerId));
            }
            return list.AsReadOnly();
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
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
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
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
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
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

        public PermissionResult CheckPermission(IRocketPlayer player, string permission)
        {
            PermissionResult result = new PermissionResult(PermissionResultType.DEFAULT, PermissionPriority.LOWEST);
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
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
            foreach (var prov in R.Providers.GetProviders<IRocketPermissionsDataProvider>())
            {
                var provResult = prov.CheckPermission(id, permission);
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
    }
}