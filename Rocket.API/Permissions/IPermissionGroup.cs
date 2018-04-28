namespace Rocket.API.Permissions
{
    /// <summary>
    ///     A permission group contains a collection of permissions.
    /// </summary>
    public interface IPermissionGroup : IPermissible
    {
        /// <summary>
        ///     The priority of this group.
        /// </summary>
        int Priority { get; }
    }
}