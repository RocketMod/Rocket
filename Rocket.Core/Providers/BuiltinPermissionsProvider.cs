using Rocket.API.Player;
using System;
using Rocket.API.Assets;
using System.Collections.ObjectModel;
using Rocket.API.Providers;
using Rocket.API.Serialization;
using Rocket.API.Permissions;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Logging;
using Rocket.Core.Providers;

namespace Rocket.Core.Permissions
{
   
    public sealed class BuiltinPermissionsProvider : ProviderBase, IPermissionsProvider
    {
        internal Asset<RocketPermissions> permissions = null;
        
        private ILoggingProvider Logger;

        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(IPlayer player)
        {
            return GetGroups(player).AsReadOnly();
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(string playerId)
        {
            return GetGroups(playerId).AsReadOnly();
        }

        public List<RocketPermissionsGroup> GetGroupsByIds(List<string> ids)
        {
            return permissions.Instance.Groups.Where(g => ids.Select(i => i.ToLower()).Contains(g.Id.ToLower())).ToList();
        }

        public List<string> GetParentGroups(string parentGroup, string currentGroup)
        {
            List<string> allgroups = new List<string>();
            RocketPermissionsGroup group = permissions.Instance.Groups.FirstOrDefault(gr => (String.Compare(gr.Id, parentGroup, true) == 0));
            if (group != null && (String.Compare(currentGroup, group.Id, true) != 0))
            {
                allgroups.Add(group.Id);
                allgroups.AddRange(GetParentGroups(group.ParentGroup, currentGroup));
            }
            return allgroups;
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return permissions.Instance.Groups.FirstOrDefault(g => g.Id.ToLower() == groupId.ToLower());
        }

        internal bool RemovePlayerFromGroup(string groupId, IPlayer player)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return false;

            if (g.Members.FirstOrDefault(m => m == player.Id) == null) return false;

            g.Members.Remove(player.Id);
            SaveGroup(g);
            return true;
        }

        internal bool AddPlayerToGroup(string groupId, IPlayer player)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return false;

            if (g.Members.FirstOrDefault(m => m == player.Id) == null)
            {
                g.Members.Add(player.Id);
                SaveGroup(g);
            }
            return true;
        }

        public bool DeleteGroup(RocketPermissionsGroup group)
        {
            permissions.Instance.Groups.Remove(group);
            permissions.Save();
            return true;
        }

        public bool SaveGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i < 0) return false;
            permissions.Instance.Groups[i] = group;
            permissions.Save();
            return true;
        }

        public bool AddGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i != -1) return false;
            permissions.Instance.Groups.Add(group);
            permissions.Save();
            return true;
        }

        public List<RocketPermissionsGroup> GetGroups(IPlayer player)
        {
            return GetGroups(player.Id);
        }

        public List<RocketPermissionsGroup> GetGroups(string id)
        {
            List<RocketPermissionsGroup> groups = permissions.Instance.Groups.Where(g => g.Members.Contains(id)).ToList(); // Get my Groups

            RocketPermissionsGroup defaultGroup = permissions.Instance.Groups.FirstOrDefault(g => (String.Compare(g.Id, permissions.Instance.DefaultGroup, true) == 0));
            if (defaultGroup != null) groups.Add(defaultGroup);

            List<RocketPermissionsGroup> parentGroups = new List<RocketPermissionsGroup>();
            foreach (RocketPermissionsGroup g in groups)
            {
                parentGroups.AddRange(GetGroupsByIds(GetParentGroups(g.ParentGroup, g.Id)));
            }
            groups.AddRange(parentGroups);

            return groups.Distinct().OrderByDescending(g => g.Priority).ToList();
        }

        public List<string> GetPermissions(IPlayer player)
        {
            return GetPermissions(player.Id);
        }

        public List<string> GetPermissions(string id)
        {
            List<string> p = new List<string>();

            List<RocketPermissionsGroup> myGroups = GetGroups(id);

            foreach (RocketPermissionsGroup g in myGroups)
            {
                p.AddRange(g.Permissions);
            }

            return p.Distinct().ToList();
        }

        public void Load()
        {
            try
            {
                this.permissions = new XMLFileAsset<RocketPermissions>(PermissionFile);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.FATAL, ex);
            }
        }

        public string PermissionFile => "Permissions.config.xml";

        public PermissionResult CheckPermission(IPlayer player, string permission)
        {
            PermissionResult result = new PermissionResult(PermissionResultType.DEFAULT, PermissionPriority.LOW);

            foreach (var perm in GetPermissions(player))
            {
                if (perm.Equals(permission, StringComparison.OrdinalIgnoreCase) && result.Result != PermissionResultType.DENY)
                {
                    result.Result = PermissionResultType.GRANT;
                }

                if (perm.Substring(1).Equals(permission, StringComparison.OrdinalIgnoreCase) && perm.StartsWith("!"))
                {
                    result.Result = PermissionResultType.DENY;
                }
            }

            return result;
        }

        public PermissionResult CheckPermission(string id, string permission)
        {
            PermissionResult result = new PermissionResult(PermissionResultType.DEFAULT, PermissionPriority.LOW);
            
            foreach (var perm in GetPermissions(id))
            {
                if (perm.Equals(permission, StringComparison.OrdinalIgnoreCase) && result.Result != PermissionResultType.DENY)
                {
                    result.Result = PermissionResultType.GRANT;        
                }

                if (perm.Substring(1).Equals(permission, StringComparison.OrdinalIgnoreCase) && perm.StartsWith("!"))
                {
                    result.Result = PermissionResultType.DENY;
                }
            }

            return result;
        }

        public void Save()
        {
            permissions.Save();
        }

        protected override void OnLoad(IProviderManager providerManager)
        {
            Logger = providerManager.GetProvider<ILoggingProvider>();
        }
    }
}