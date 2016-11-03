using Rocket.API.Commands;
using Rocket.API.Serialisation;
using Rocket.Core;
using System.Collections.Generic;

namespace Rocket.API
{
    public static class IRocketPlayerExtension
    {
        public static bool HasPermission(this IRocketPlayer player, string permission)
        {
            return R.Permissions.HasPermission(player, permission) || player.IsAdmin;
        }

        public static bool HasPermissions(this IRocketPlayer player, IRocketCommand command)
        {
            return R.Permissions.HasPermission(player, command) || player.IsAdmin;
        }

        public static List<Permission> GetPermissions(this IRocketPlayer player)
        {
            return R.Permissions.GetPermissions(player);
        }
    }
}
