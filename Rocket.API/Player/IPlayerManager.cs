using System;
using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.API.Player
{
    /// <summary>
    ///     The service responsible for managing players.
    /// </summary>
    public interface IPlayerManager
    {
        /// <summary>
        ///     Gets all online players.
        /// </summary>
        IEnumerable<IOnlinePlayer> OnlinePlayers { get; }

        /// <summary>
        ///     Kicks (disconnects) the given player from the server.
        /// </summary>
        /// <param name="player">The player to kick.</param>
        /// <param name="caller">The command caller which kicks the player (optional).</param>
        /// <param name="reason">The kick reason whicht might be shown to the player (optional).</param>
        /// <returns><b>true</b> if the player could be kicked; otherwise, <b>false</b>.</returns>
        bool Kick(IOnlinePlayer player, ICommandCaller caller = null, string reason = null);

        /// <summary>
        ///     Bans the given player from the server.
        /// </summary>
        /// <param name="player">The player to ban.</param>
        /// <param name="caller">The command caller which bans the player (optional).</param>
        /// <param name="reason">The ban reason which might be shown to the player (optional).</param>
        /// <param name="timeSpan">The ban duration. Will never expire if null.</param>
        /// <returns><b>true</b> if the player could be banned; otherwise, <b>false</b>.</returns>
        bool Ban(IPlayer player, ICommandCaller caller = null, string reason = null, TimeSpan? timeSpan = null);

        /// <summary>
        ///     Unbans the given player from the server.
        /// </summary>
        /// <param name="player">The player to unban.</param>
        /// <param name="caller">The command caller which unbans the player.</param>
        /// <returns><b>true</b> if the player could be unbanned; otherwise, <b>false</b>.</returns>
        bool Unban(IPlayer player, ICommandCaller caller = null);

        /// <summary>
        /// Gets an online player by name or id.
        /// </summary>
        /// <param name="nameOrId">The name or id of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerNotFoundException">When the player was not found.</exception>
        IOnlinePlayer GetOnlinePlayer(string nameOrId);

        /// <summary>
        /// Gets an online player by name.
        /// </summary>
        /// <param name="name">The name of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerNameNotFoundException">When the player was not found.</exception>

        IOnlinePlayer GetOnlinePlayerByName(string name);

        /// <summary>
        /// Gets an online player by id.
        /// </summary>
        /// <param name="id">The id of the online player to get.</param>
        /// <returns>The online players instance.</returns>
        /// <exception cref="PlayerIdNotFoundException">When the player was not found.</exception>
        IOnlinePlayer GetOnlinePlayerById(string id);

        /// <summary>
        ///     Tries to get an online player by name or id.
        /// </summary>
        /// <param name="nameOrId">The name or id of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayer(string nameOrId, out IOnlinePlayer output);

        /// <summary>
        ///     Tries to get an online player by id.
        /// </summary>
        /// <param name="id">The id of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayerById(string id, out IOnlinePlayer output);

        /// <summary>
        ///     Tries to get an online player by name.
        /// </summary>
        /// <param name="name">The name of the player to get.</param>
        /// <param name="output">The players instance if the player was found and is online; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the player was found and is online; otherwise, <b>false</b>.</returns>
        bool TryGetOnlinePlayerByName(string name, out IOnlinePlayer output);

        /// <summary>
        ///     Gets an online or offline players instance.<br/><br/>
        ///     <b>It is guaranteed to return <see cref="IOnlinePlayer"/> instances for online players.</b><br/><br/>
        ///     <b>This method will never return null, even if the player never connected to the server.</b> You can use
        ///     <see cref="IPlayer.LastSeen"/> to check if the player was ever online.
        /// </summary>
        /// <param name="id">The id of the player to get.</param>
        /// <returns>The player instance.</returns>
        IPlayer GetPlayer(string id);
    }
}