using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core.Permissions
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
            RocketPermissionsGroup group = permissions.Instance.Groups.Where(gr => (String.Compare(gr.Id, parentGroup, true) == 0)).FirstOrDefault();
            if (group != null && (String.Compare(currentGroup, group.Id, true) != 0))
            {
                allgroups.Add(group.Id);
                allgroups.AddRange(GetParentGroups(group.ParentGroup, currentGroup));
            }
            return allgroups;
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            List<Permission> applyingPermissions = GetPermissions(player, requestedPermissions);

            if (applyingPermissions.Count != 0 )
            {
                return true;
            }
            return false;
        }

        internal RocketPermissionsGroup GetGroup(string groupId)
        {
            return permissions.Instance.Groups.Where(g => g.Id.ToLower() == groupId.ToLower()).FirstOrDefault();
        }

        internal RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return RocketPermissionsProviderResult.GroupNotFound;

            if (g.Members.Where(m => m == player.Id).FirstOrDefault() == null) return RocketPermissionsProviderResult.PlayerNotFound;

            g.Members.Remove(player.Id);
            SaveGroup(g);
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return RocketPermissionsProviderResult.GroupNotFound;

            if (g.Members.Where(m => m == player.Id).FirstOrDefault() != null) return RocketPermissionsProviderResult.DuplicateEntry;

            g.Members.Add(player.Id);
            SaveGroup(g);
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return RocketPermissionsProviderResult.GroupNotFound;

            permissions.Instance.Groups.Remove(g); 
            permissions.Save();
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i < 0) return RocketPermissionsProviderResult.GroupNotFound;
            permissions.Instance.Groups[i] = group;
            permissions.Save();
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i != -1) return RocketPermissionsProviderResult.DuplicateEntry;
            permissions.Instance.Groups.Add(group);
            permissions.Save();
            return RocketPermissionsProviderResult.Success;
        }


        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            List<RocketPermissionsGroup> groups = permissions.Instance.Groups.Where(g => g.Members.Contains(player.Id)).ToList(); // Get my Groups
            RocketPermissionsGroup defaultGroup = permissions.Instance.Groups.Where(g => (String.Compare(g.Id, permissions.Instance.DefaultGroup, true) == 0)).FirstOrDefault();
            if (defaultGroup != null) groups.Add(defaultGroup);

            if (includeParentGroups)
            {
                List<RocketPermissionsGroup> parentGroups = new List<RocketPermissionsGroup>();
                foreach (RocketPermissionsGroup g in groups)
                {
                    parentGroups.AddRange(GetGroupsByIds(GetParentGroups(g.ParentGroup, g.Id)));
                }
                groups.AddRange(parentGroups);
            }

            return groups.Distinct().ToList();
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            List<Permission> p = new List<Permission>();

            List<RocketPermissionsGroup> myGroups = GetGroups(player, true);

            foreach (RocketPermissionsGroup g in myGroups)
            {
                p.AddRange(g.Permissions);
            }

            return p.Distinct().ToList();
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            List<Permission> playerPermissions = GetPermissions(player);
            playerPermissions.ForEach((Permission p) => { p.Name = p.Name.ToLower(); });

            List<Permission> applyingPermissions = playerPermissions.Where(p => requestedPermissions.Contains(p.Name)).ToList();


            if (playerPermissions.Where(p => p.Name == "*").FirstOrDefault() != null) applyingPermissions.Add(new Permission("*"));

            foreach (Permission p in playerPermissions)
            {
                string pb = p.Name;
                if (pb.Contains(".")) pb = p.Name.Substring(0, p.Name.IndexOf('.'));

                if (p.Name.EndsWith(".*")) //Player permission is a wildcard permission
                {
                    foreach (string ps in requestedPermissions)
                    {
                        string b = ps;
                        if (ps.Contains("."))
                            b = ps.Substring(0, ps.IndexOf('.')).ToLower();

                        if (ps.StartsWith(pb + ".")) //Check if wildcard base pb is the start of this permission
                        {
                            applyingPermissions.Add(p);
                        }
                    }
                }

                //Grant base permission if required
                requestedPermissions.Where(ps => ps == pb).ToList().ForEach((ap) => { applyingPermissions.Add(p); });
            }
            return applyingPermissions;
        }

    }
}