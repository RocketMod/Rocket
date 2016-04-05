using Rocket.API.Serialisation;
using Rocket.Core;
using System.Collections.Generic;

namespace Rocket.API
{
    public static class IRocketPlayerExtension
    {
        public static bool HasPermission(this IRocketPlayer player, string permission)
        {
            return R.Permissions.HasPermission(player, permission, player.IsAdmin);
        }

        public static bool HasPermissions(this IRocketPlayer player, List<string> permissions)
        {
            uint? cooldownLeft;
            return R.Permissions.HasPermission(player, permissions,out cooldownLeft, player.IsAdmin);
        }

        public static List<Permission> GetPermissions(this IRocketPlayer player)
        {
            if (player.IsAdmin) return new List<Permission>() { new Permission("*") };
            return R.Permissions.GetPermissions(player);
        }
    }
}
