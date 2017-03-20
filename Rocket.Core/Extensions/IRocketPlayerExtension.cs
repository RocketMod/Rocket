using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Core.Extensions
{
    public static class IRocketPlayerExtension
    {
        public static bool HasPermission(this IRocketPlayer player, string permission)
        {
            return R.Permissions.HasPermission(player, permission);
        }

        public static bool HasPermissions(this IRocketPlayer player, IRocketCommand command)
        {
            return R.Permissions.HasPermission(player, command) ;
        }

        public static List<string> GetPermissions(this IRocketPlayer player)
        {
            return R.Permissions.GetPermissions(player);
        }
    }
}
