using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Core.Permissions
{
    public class PermissionProvider : IPermissionProvider
    {
        public bool HasPermissions(IPermissionable permissionable)
        {
            return true;
        }
    }
}
