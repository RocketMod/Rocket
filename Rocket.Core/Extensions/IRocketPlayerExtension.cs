using Rocket.API.Commands;
using Rocket.API.Providers;
using Rocket.API.Serialisation;
using Rocket.Core;
using System.Collections.Generic;

namespace Rocket.API
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
