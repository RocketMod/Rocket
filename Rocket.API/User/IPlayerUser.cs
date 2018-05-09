using Rocket.API.Player;

namespace Rocket.API.User
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