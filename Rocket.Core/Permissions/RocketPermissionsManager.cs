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
        private RocketPermissionsHelper helper;

        private void Start()
        {
            try
            {
                if (R.Settings.Instance.WebPermissions.Enabled)
                {
                    lastWebPermissionsUpdate = DateTime.Now;
                    helper = new RocketPermissionsHelper(new WebXMLFileAsset<RocketPermissions>(R.Settings.Instance.WebPermissions.Url + "?instance=" + R.Implementation.InstanceId));
                    updateWebPermissions = true;
                }
                else
                {
                    helper = new RocketPermissionsHelper(new XMLFileAsset<RocketPermissions>(Environment.PermissionFile));
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
                    helper.permissions.Load((IAsset<RocketPermissions> asset) => {
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
            helper.permissions.Reload();
        }

        public bool HasPermission(IRocketPlayer player, string permission, bool defaultReturnValue = false)
        {
            return helper.HasPermission(player, permission, defaultReturnValue);
        }

        public bool HasPermission(IRocketPlayer player, IRocketCommand command, out uint? cooldownLeft, bool defaultReturnValue = false)
        {
            return helper.HasPermission(player, command,out cooldownLeft, defaultReturnValue);
        }

        public bool HasPermission(IRocketPlayer player, List<string> permissions, out uint? cooldownLeft, bool defaultReturnValue = false)
        {
            return helper.HasPermission(player, permissions, out cooldownLeft, defaultReturnValue);
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            return helper.GetGroups(player, includeParentGroups);
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            return helper.GetPermissions(player);
        }

        public bool SetGroup(IRocketPlayer player, string groupID)
        {
            return helper.SetGroup(player,groupID);
        }
    }
}