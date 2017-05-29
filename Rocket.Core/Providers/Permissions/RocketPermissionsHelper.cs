using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Assets;
using Rocket.API.Player;
using Rocket.API.Serialisation;

namespace Rocket.Core.Providers.Permissions
{
    internal class RocketPermissionsHelper
    {
        internal Asset<RocketPermissions> permissions = null;

        public RocketPermissionsHelper(Asset<RocketPermissions> permissions)
        {
            this.permissions = permissions;
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

        internal RocketPermissionsGroup GetGroup(string groupId)
        {
            return permissions.Instance.Groups.FirstOrDefault(g => g.Id.ToLower() == groupId.ToLower());
        }

        internal bool RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return false;

            if (g.Members.FirstOrDefault(m => m == player.Id) == null) return false;

            g.Members.Remove(player.Id);
            SaveGroup(g);
            return true;
        }

        internal bool AddPlayerToGroup(string groupId, IRocketPlayer player)
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

        internal bool DeleteGroup(RocketPermissionsGroup group)
        {
            permissions.Instance.Groups.Remove(group);
            permissions.Save();
            return true;
        }

        internal bool SaveGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i < 0) return false;
            permissions.Instance.Groups[i] = group;
            permissions.Save();
            return true;
        }

        internal bool AddGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i != -1) return false;
            permissions.Instance.Groups.Add(group);
            permissions.Save();
            return true;
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player)
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

        public List<string> GetPermissions(IRocketPlayer player)
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
    }
}