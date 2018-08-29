using System.Collections.Generic;
using Rocket.API.User;

namespace Rocket.API.Player
{
    /// <summary>
    ///     The service responsible for managing players.
    /// </summary>
    public interface IPlayerManager : IUserManager
    {
        /// <summary>
        ///     Kicks (disconnects) the given user from the server.
        /// </summary>
        /// <param name="user">The user to kick.</param>
        /// <param name="kickedBy">The user which kicks (optional).</param>
        /// <param name="reason">The kick reason whicht might be shown to the user (optional).</param>
        /// <returns><b>true</b> if the user could be kicked; otherwise, <b>false</b>.</returns>
        bool Kick(IPlayer user, IUser kickedBy = null, string reason = null);

        /// <summary>
        ///     Gets all online players.
        /// </summary>
        IEnumerable<IPlayer> Players { get; }

        /// <summary>
        ///     Gets an online player by name or id.
        /// </summary>
        /// <param name="nameOrId">The name or id of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerNotFoundException">When the player was not found.</exception>
        IPlayer GetOnlinePlayer(string nameOrId);

        /// <summary>
        ///     Gets an online player by name.
        /// </summary>
        /// <param name="name">The name of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerNameNotFoundException">When the player was not found.</exception>
        IPlayer GetOnlinePlayerByName(string name);

        /// <summary>
        ///     Gets an online player by id.
        /// </summary>
        /// <param name="id">The id of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerIdNotFoundException">When the player was not found.</exception>
        IPlayer GetOnlinePlayerById(string id);

        /// <summary>
        ///     Tries to get an online player by name or id.
        /// </summary>
        /// <param name="nameOrId">The name or id of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayer(string nameOrId, out IPlayer output);

        /// <summary>
        ///     Tries to get an online player by id.
        /// </summary>
        /// <param name="id">The id of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayerById(string id, out IPlayer output);

        /// <summary>
        ///     Tries to get an online player by name.
        /// </summary>
        /// <param name="name">The name of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayerByName(string name, out IPlayer output);

        /// <summary>
        ///     Gets an online or offline players instance.<br /><br />
        ///     <b>This method will never return null, even if the player never connected to the server.</b> You can use
        ///     <see cref="IUser.LastSeen" /> to check if the player was ever online.
        /// </summary>
        /// <param name="id">The id of the player to get.</param>
        /// <returns>The player instance.</returns>
        IPlayer GetPlayer(string id);
    }
}