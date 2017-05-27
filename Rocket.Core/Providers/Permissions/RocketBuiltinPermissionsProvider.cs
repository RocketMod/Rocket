using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Permissions;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using System;
using System.Collections.Generic;
using Rocket.API.Assets;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rocket.Core.Providers.Permissions
{
    public sealed class RocketBuiltinPermissionsProvider : IRocketPermissionsDataProvider
    {
        private RocketPermissionsHelper helper;
        
        public ReadOnlyCollection<RocketPermissionsGroup> GetGroups(IRocketPlayer player)
        {
            return helper.GetGroups(player).AsReadOnly();
        }

        public ReadOnlyCollection<string> GetPermissions(IRocketPlayer player)
        {
            return helper.GetPermissions(player).AsReadOnly();
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return helper.GetGroup(groupId);
        }

        public bool AddGroup(RocketPermissionsGroup group)
        {
            return helper.AddGroup(group);
        }

        public bool SaveGroup(RocketPermissionsGroup group)
        {
            return helper.SaveGroup(group);
        }

        public bool DeleteGroup(RocketPermissionsGroup group)
        {
            return helper.DeleteGroup(group);
        }
        
        public void Save()
        {
            helper.permissions.Save();
        }

        public void Unload(bool isReload = false)
        {
            helper.permissions.Unload();
        }

        public void Load(bool reloading)
        {
            try
            {
                helper = new RocketPermissionsHelper(new XMLFileAsset<RocketPermissions>(PermissionFile));
            }
            catch (Exception ex)
            {
                R.Logger.Fatal(ex);
            }
        }

        public string PermissionFile => "Permissions.config.xml";

        public bool HasPermission(IRocketPlayer player, string permission)
        {
            //todo ??
            return
                GetPermissions(player)
                    .Any(c => c.Equals(permission, StringComparison.OrdinalIgnoreCase) && !c.StartsWith("!"));
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetGroups(string id)
        {
            return helper.GetGroups(id).AsReadOnly();
        }

        public ReadOnlyCollection<string> GetPermissions(string id)
        {
            return helper.GetPermissions(id).AsReadOnly();
        }

        public bool HasPermission(string id, string permission)
        {
            return
                GetPermissions(id)
                    .Any(c => c.Equals(permission, StringComparison.OrdinalIgnoreCase) && !c.StartsWith("!"));
        }
    }
}