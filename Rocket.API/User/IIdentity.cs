namespace Rocket.API.User
{
    /// <summary>
    ///     Defines the identity's types.
    /// </summary>
    public enum IdentityType
    {
        /// <summary>
        ///     A console identity.
        /// </summary>
        Console = 0,

        /// <summary>
        ///     A player identitiy.
        /// </summary>
        Player = 1,

        /// <summary>
        ///     A group identity.
        /// </summary>
        Group = 2,

        /// <summary>
        ///     A custom identity.
        /// </summary>
        Custom = 3
    }

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
        ///     The name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The identity type.
        /// </summary>
        IdentityType Type { get; }
    }
}