using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Core.Permissions
{
    public sealed class RocketPermissionsManager : MonoBehaviour, IRocketPermissionsProvider
    {
        private RocketPermissionsHelper helper;

        private void Start()
        {
            try
            {
                if (R.Instance.Settings.Instance.WebPermissions.Enabled)
                {
                    lastWebPermissionsUpdate = DateTime.Now;
                    helper = new RocketPermissionsHelper(new WebXMLFileAsset<RocketPermissions>(new Uri(R.Instance.Settings.Instance.WebPermissions.Url + "?instance=" + R.Implementation.InstanceId)));
                    updateWebPermissions = true;
                }
                else
                {
                    helper = new RocketPermissionsHelper(new XMLFileAsset<RocketPermissions>(Environment.PermissionFile));
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        private bool updateWebPermissions = false;
        private DateTime lastWebPermissionsUpdate;

        private void FixedUpdate()
        {
            try
            {
                if (updateWebPermissions && R.Instance.Settings.Instance.WebPermissions.Interval > 0 && (DateTime.Now - lastWebPermissionsUpdate) > TimeSpan.FromSeconds(R.Instance.Settings.Instance.WebPermissions.Interval))
                {
                    lastWebPermissionsUpdate = DateTime.Now;
                    updateWebPermissions = false;
                    helper.permissions.Load((IAsset<RocketPermissions> asset) =>
                    {
                        updateWebPermissions = true;
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        public void Reload()
        {
            helper.permissions.Load();
        }

        public bool HasPermission(IRocketPlayer player, string permission)
        {
            return helper.HasPermission(player, new List<string>() { permission });
        }

        public bool HasPermission(IRocketPlayer player, IRocketCommand command)
        {
            List<string> permissions = new List<string>();
            permissions.Add(command.Name);
            permissions.AddRange(command.Aliases);
            permissions.AddRange(command.Permissions);
            return helper.HasPermission(player, permissions);
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            return helper.GetGroups(player, includeParentGroups);
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            return helper.GetPermissions(player);
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            return helper.GetPermissions(player, requestedPermissions);
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            return helper.AddPlayerToGroup(groupId, player);
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            return helper.RemovePlayerFromGroup(groupId, player);
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return helper.GetGroup(groupId);
        }

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            return helper.SaveGroup(group);
        }

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            return helper.AddGroup(group);
        }

        public RocketPermissionsProviderResult DeleteGroup(RocketPermissionsGroup group)
        {
            return helper.DeleteGroup(group.Id);
        }

        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            return helper.DeleteGroup(groupId);
        }
    }
}