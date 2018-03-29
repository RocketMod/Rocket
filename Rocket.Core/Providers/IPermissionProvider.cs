using Rocket.API.Player;

namespace Rocket.Providers
{
    //Todo: Going to need a PermissionGroup object.
    public interface IPermissionProvider
    {
        bool HasPermissions(IPlayer player);
    }
}
