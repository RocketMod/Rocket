using Rocket.API.Permissions;

namespace Rocket.Core.Permissions {
    public class PermissionProvider : IPermissionProvider {
        public bool HasPermissions(IIdentifiable identifiable) {
            return true;
        }
    }
}