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
        internal List<PermissionCooldown> cooldown = new List<PermissionCooldown>();

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

        public bool HasPermission(IRocketPlayer player, string permission, bool defaultReturnValue = false)
        {
            uint? cooldownLeft;
            return HasPermission(player, new List<string>() { permission }, out cooldownLeft, defaultReturnValue);
        }

        public bool HasPermission(IRocketPlayer player, IRocketCommand command, out uint? cooldownLeft, bool defaultReturnValue = false)
        {
            List<string> commandPermissions = command.Permissions;
            commandPermissions.Add(command.Name);
            commandPermissions.AddRange(command.Aliases);
            commandPermissions = commandPermissions.Select(a => a.ToLower()).ToList();
            return HasPermission(player, commandPermissions, out cooldownLeft, defaultReturnValue);
        }

        public bool HasPermission(IRocketPlayer player, List<string> permissions, out uint? cooldownLeft, bool defaultReturnValue = false)
        {
            cooldownLeft = null;
            List<Permission> playerPermissions = GetPermissions(player);
            playerPermissions.ForEach((Permission p) => { p.Name = p.Name.ToLower(); });


            List<Permission> applyingPermissions = playerPermissions.Where(p => permissions.Contains(p.Name)).ToList();
            List<Permission> inheritedPermission = playerPermissions.Where(p => permissions.Where(c => p.Name.StartsWith(c + ".")).FirstOrDefault() != null).ToList(); //Allow kit to be executed when kit.vip is assigned



            if (playerPermissions.Exists(e => e.Name == "*") || applyingPermissions.Count != 0 || inheritedPermission.Count != 0)
            {
                //Has permissions
                Permission cooldownPermission = applyingPermissions.Where(p => p.Cooldown != 0).OrderByDescending(p => p.Cooldown).FirstOrDefault();
                if (cooldownPermission != null)
                {
                    //Has a cooldown
                    uint? cd = cooldownPermission.Cooldown;

                    PermissionCooldown pc = cooldown.Where(c => c.Player.Id == player.Id && c.Permission == cooldownPermission).FirstOrDefault();
                    if (pc == null)
                    {
                        //Is in cooldown list
                        cooldown.Add(new PermissionCooldown(player, cooldownPermission));
                    }
                    else
                    {
                        double timeSinceExecution = (DateTime.Now - pc.PermissionRequested).TotalSeconds;
                        if (pc.Permission.Cooldown <= timeSinceExecution)
                        {
                            //Cooldown has it expired
                            cooldown.Remove(pc);
                        }
                        else
                        {
                            cooldownLeft = pc.Permission.Cooldown - (uint)timeSinceExecution;
                            return false;
                        }
                    }
                }
                return true;
            }

            return defaultReturnValue;
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

        public bool SetGroup(IRocketPlayer player, string groupID)
        {
            bool added = false;
            foreach (RocketPermissionsGroup g in permissions.Instance.Groups)
            {
                if (g.Members.Contains(player.Id))
                {
                    g.Members.Remove(player.Id);
                }

                if (String.Compare(g.Id, groupID, true) == 0)
                {
                    g.Members.Add(player.Id);
                    added = true;
                }
            }
            if (added)
            {
                permissions.Save();
                return true;
            }
            return false;
        }
    }
}