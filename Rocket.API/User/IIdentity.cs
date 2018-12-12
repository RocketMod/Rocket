namespace Rocket.API.User
{
    /// <summary>
    ///     An identity.
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        ///     The unique id.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The identity type (e.g. "rocket", "steam", etc.)
        /// </summary>
        string IdentityType { get; }
    }
}