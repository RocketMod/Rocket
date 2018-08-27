using Rocket.API.User;

namespace Rocket.API.Permissions
{
    /// <summary>
    ///     A permission group contains a collection of permissions.
    /// </summary>
    public interface IPermissionGroup : IPermissionEntity
    {
        /// <summary>
        ///     The priority of this group.
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     Defines if the group should be auto assigned.
        /// </summary>
        bool AutoAssign { get; }

        string Name { get; }
    }
}