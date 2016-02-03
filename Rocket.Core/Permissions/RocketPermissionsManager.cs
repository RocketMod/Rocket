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
                    }, true);
                }
            }

            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void Reload()
        {
            permissions.Reload();
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


        public bool HasPermission(IRocketPlayer player, string requestedPermission, bool defaultReturnValue = false)
        {
            uint? cooldownLeft;
            return HasPermission(player, requestedPermission,out cooldownLeft, defaultReturnValue);
        }

        public bool HasPermission(IRocketPlayer player, string requestedPermission,out uint? cooldownLeft, bool defaultReturnValue = false)
        {
            cooldownLeft = null;
            List<Permission> permissions = R.Permissions.GetPermissions(player);

            List<Permission> applyingPermissions = permissions.Where(p => p == requestedPermission).ToList();

            if(permissions.Contains("*") || applyingPermissions.Count != 0)
            {
                //Has permissions

                Permission cooldownPermission = applyingPermissions.Where(p => p != null).OrderByDescending(p => p.Cooldown).FirstOrDefault();
                if(cooldownPermission != null) 
                {
                    //Has a cooldown
                    uint? cd = cooldownPermission.Cooldown;

                    PermissionCooldown pc = cooldown.Where(c => c.Player == player && c.Permission == cooldownPermission).FirstOrDefault();
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
                            cooldownLeft = (uint)timeSinceExecution;
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
            RocketPermissionsGroup defaultGroup = permissions.Instance.Groups.Where(g => (String.Compare(g.Id, permissions.Instance.DefaultGroupId, true) == 0)).FirstOrDefault();
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
                foreach (RocketPermissionsGroup myGroup in permissions.Instance.Groups.Where(group => group.ParentGroup.Contains(g.Id.ToLower())))
                {
                    if (myGroup.Members.Contains(player.Id))
                        p.AddRange(myGroup.Commands);
                }
                p.AddRange(g.Commands);
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