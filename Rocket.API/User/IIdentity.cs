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
        ///     The name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The identity type.
        /// </summary>
        string IdentityType { get; }
    }

    /// <summary>
    ///     Defines built in Rocket identity types.
    /// </summary>
    public static class IdentityTypes
    {
        /// <summary>
        ///     A console identity.
        /// </summary>
        public static readonly string Console = "console";

        /// <summary>
        ///     A player identitiy.
        /// </summary>
        public static readonly string Player = "player";
        /// <summary>
        ///     A group identity.
        /// </summary>
        public static readonly string Group = "group";
    }
}