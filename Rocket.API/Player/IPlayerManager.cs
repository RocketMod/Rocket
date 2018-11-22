using Rocket.API.User;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<bool> KickAsync(IPlayer user, IUser kickedBy = null, string reason = null);

        /// <summary>
        ///     Gets all online players.
        /// </summary>
        Task<IEnumerable<IPlayer>> GetPlayersAsync();

        /// <summary>
        ///     Gets a player by name or id.
        /// </summary>
        /// <param name="nameOrId">The name or id of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerNotFoundException">When the player was not found.</exception>
        Task<IPlayer> GetPlayerAsync(string nameOrId);

        /// <summary>
        ///     Gets a player by name.
        /// </summary>
        /// <param name="name">The name of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerNameNotFoundException">When the player was not found.</exception>
        Task<IPlayer> GetPlayerByNameAsync(string name);

        /// <summary>
        ///     Gets a player by id.
        /// </summary>
        /// <param name="id">The id of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerIdNotFoundException">When the player was not found.</exception>
        Task<IPlayer> GetPlayerByIdAsync(string id);

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
        /// <param name="playerId">The id of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayerById(string playerId, out IPlayer output);

        /// <summary>
        ///     Tries to get an online player by name.
        /// </summary>
        /// <param name="name">The name of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayerByName(string name, out IPlayer output);
    }
}