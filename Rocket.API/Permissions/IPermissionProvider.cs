using Rocket.API.Player;

namespace Rocket.API.Permissions
{
    public interface IPermissionProvider
    {
        bool HasPermissions(IPlayer player);
    }
}