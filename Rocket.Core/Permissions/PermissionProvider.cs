using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Core.Permissions
{
    public class PermissionProvider : IPermissionProvider
    {
        public bool HasPermissions(IIdentifiable identifiable)
        {
            return true;
        }
    }
}
