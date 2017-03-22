using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Permissions;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using System;
using System.Collections.Generic;

namespace Rocket.Core.Providers.Permissions
{
    public sealed class RocketBuiltinPermissionsProvider : RocketProviderBase,IRocketPermissionsDataProvider
    {
        private RocketPermissionsHelper helper;
        
        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player)
        {
            return helper.GetGroups(player);
        }

        public List<string> GetPermissions(IRocketPlayer player)
        {
            return helper.GetPermissions(player);
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

        public void Reload()
        {
            helper.permissions.Load();
        }

        public void Save()
        {
            helper.permissions.Save();
        }

        public override void Unload()
        {
            helper.permissions.Unload();
        }

        public void Load()
        {
            try
            {
                helper = new RocketPermissionsHelper(new XMLFileAsset<RocketPermissions>(API.Environment.PermissionFile));
            }
            catch (Exception ex)
            {
                R.Logger.Fatal(ex);
            }
        }
    }
}