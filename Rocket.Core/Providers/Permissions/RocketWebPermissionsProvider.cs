using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Commands;
using Rocket.API.Providers;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Core.Providers
{
    public sealed class RocketWebPermissionsProvider : MonoBehaviour, IRocketPermissionsDataProvider
    {
        private RocketPermissionsHelper helper;

        private bool updateWebPermissions = false;
        private DateTime lastWebPermissionsUpdate;

        private void Start()
        {
            try
            {
                lastWebPermissionsUpdate = DateTime.Now;
                helper = new RocketPermissionsHelper(new WebXMLFileAsset<RocketPermissions>(new Uri(R.Settings.Instance.WebPermissions.Url + "?instance=" + R.Implementation.InstanceName)));
                updateWebPermissions = true;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }


        private void FixedUpdate()
        {
            try
            {
                if (updateWebPermissions && R.Settings.Instance.WebPermissions.Interval > 0 && (DateTime.Now - lastWebPermissionsUpdate) > TimeSpan.FromSeconds(R.Settings.Instance.WebPermissions.Interval))
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
            throw new NotImplementedException();
        }

        public bool DeleteGroup(RocketPermissionsGroup group)
        {
            throw new NotImplementedException();
        }

        public void Reload()
        {
            helper.permissions.Load();
        }
    }
}