using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rocket.Core.Permissions
{
    public sealed class RocketPermissionsManager : MonoBehaviour, IRocketPermissionsProvider
    {
        private Asset<RocketPermissions> permissions = null;
        private List<PermissionCooldown> cooldown = new List<PermissionCooldown>();

        private void Start()
        {
            try
            {
                if (R.Settings.Instance.WebPermissions.Enabled)
                {
                    lastWebPermissionsUpdate = DateTime.Now;
                    permissions = new WebXMLFileAsset<RocketPermissions>(R.Settings.Instance.WebPermissions.Url + "?instance=" + R.Implementation.InstanceId);
                    updateWebPermissions = true;
                }
                else
                {
                    permissions = new XMLFileAsset<RocketPermissions>(Environment.PermissionFile);
                }

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        
        private bool updateWebPermissions = false;
        private DateTime lastWebPermissionsUpdate;

        private void FixedUpdate()
        {
            try
            {
                if (updateWebPermissions && R.Settings.Instance.WebPermissions.Interval > 0 && (DateTime.Now - lastWebPermissionsUpdate) > TimeSpan.FromSeconds(R.Settings.Instance.WebPermissions.Interval))
                {
                    lastWebPermissionsUpdate = DateTime.Now;
                    updateWebPermissions = false;
                    permissions.Load((IAsset<RocketPermissions> asset) => {
                        updateWebPermissions = true;
                    });
                }
            }

            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void Reload()
        {
            permissions.Load();
        }

        private List<RocketPermissionsGroup> getGroupsByIds(List<string> ids)
        {
            return permissions.Instance.Groups.Where(g => ids.Select(i => i.ToLower()).Contains(g.Id.ToLower())).ToList();
        }

        private List<string> getParentGroups(string parentGroup, string currentGroup)
        {
            List<string> allgroups = new List<string>();
            RocketPermissionsGroup group = permissions.Instance.Groups.Where(gr => (String.Compare(gr.Id, parentGroup, true) == 0)).FirstOrDefault();
            if (group != null && (String.Compare(currentGroup, group.Id, true) != 0))
            {
                allgroups.Add(group.Id);
                allgroups.AddRange(getParentGroups(group.ParentGroup, currentGroup));
            }
            return allgroups;
        }

        private class PermissionCooldown{
            public IRocketPlayer Player;
            public DateTime PermissionRequested;
            public Permission Permission;

            public PermissionCooldown(IRocketPlayer player, Permission permission)
            {
                Player = player;
                Permission = permission;
                PermissionRequested = DateTime.Now;
            }
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

            List<Permission> playerPermissions = R.Permissions.GetPermissions(player);
            playerPermissions.ForEach((Permission p) => { p.Name = p.Name.ToLower(); });


            List<Permission> applyingPermissions = playerPermissions.Where(p => permissions.Contains(p.Name)).ToList();
            List<Permission> inheritedPermission = playerPermissions.Where(p => permissions.Where(c => p.Name.StartsWith(c+".")).FirstOrDefault() != null).ToList(); //Allow kit to be executed when kit.vip is assigned



            if (playerPermissions.Exists(e => e.Name == "*") || applyingPermissions.Count != 0 || inheritedPermission.Count != 0)
            {
                //Has permissions
                Permission cooldownPermission = applyingPermissions.Where(p => p.Cooldown.HasValue).OrderByDescending(p => p.Cooldown).FirstOrDefault();
                if(cooldownPermission != null) 
                {
                    //Has a cooldown
                    uint? cd = cooldownPermission.Cooldown;

                    PermissionCooldown pc = cooldown.Where(c => c.Player.Id == player.Id && c.Permission == cooldownPermission).FirstOrDefault();
                    if(pc == null)
                    {
                        //Is in cooldown list
                        cooldown.Add(new PermissionCooldown(player,cooldownPermission));
                    }
                    else
                    {
                        double timeSinceExecution = (DateTime.Now - pc.PermissionRequested).TotalSeconds;
                        if (pc.Permission.Cooldown.Value <= timeSinceExecution)
                        {
                            //Cooldown has it expired
                            cooldown.Remove(pc);
                        }
                        else
                        {
                            cooldownLeft = pc.Permission.Cooldown.Value - (uint)timeSinceExecution;
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
                    parentGroups.AddRange(getGroupsByIds(getParentGroups(g.ParentGroup, g.Id)));
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