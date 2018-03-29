using Rocket.API.Player;

namespace Rocket.Core.Permissions
{
    //Todo: Going to need a PermissionGroup object.
    public interface IPermissionProvider
    {
        bool HasPermissions(IPlayer player);
    }
}
