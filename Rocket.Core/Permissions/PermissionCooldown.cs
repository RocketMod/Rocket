using Rocket.API;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Permissions
{
    internal class PermissionCooldown
    {
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

}
