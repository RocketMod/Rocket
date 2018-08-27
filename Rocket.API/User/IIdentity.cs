namespace Rocket.API.User
{
    /// <summary>
    ///     Defines built in Rocket identity types.
    /// </summary>
    public enum IdentityProvider { Builtin = 1, Steam = 2 };

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
        ///     The identity provider.
        /// </summary>
        IdentityProvider Provider { get; }
    }
}