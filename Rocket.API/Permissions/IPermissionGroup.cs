using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.API.Permissions
{
    /// <summary>
    ///     A permission group contains a collection of permissions.
    /// </summary>
    public interface IPermissionGroup : IIdentity
    {
        /// <summary>
        ///     The priority of this group.
        /// </summary>
        int Priority { get; }
    }
}