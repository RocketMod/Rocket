using Rocket.API.User;

namespace Rocket.API.Player
{
    /// <summary>
    ///     <inheritdoc cref="IPlayer"/>
    /// </summary>
    /// <typeparam name="TPlayer"></typeparam>
    public interface IPlayerUser<out TPlayer> : IPlayerUser where TPlayer: IPlayer
    {
        /// <summary>
        ///     The parent player object.
        /// </summary>
        TPlayer Player { get; }
    }

    /// <summary>
    ///     A players user.
    /// </summary>
    public interface IPlayerUser : IUser
    {

    }
}