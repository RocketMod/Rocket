using Rocket.API.User;

namespace Rocket.API.Player
{
    /// <summary>
    ///     A players user.
    /// </summary>
    public interface IPlayerUser : IUser
    {
        /// <summary>
        ///     The parent player object.
        /// </summary>
        IPlayer Player { get; }
    }
}