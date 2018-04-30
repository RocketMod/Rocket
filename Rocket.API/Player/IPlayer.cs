using System;
using Rocket.API.Permissions;

namespace Rocket.API.Player
{
    /// <summary>
    ///     Represents a game player.
    /// </summary>
    public interface IPlayer : IIdentifiable, IFormattable
    {
        /// <summary>
        ///     Defines if the player is currently online. Does not guarantee that this is an <see cref="IOnlinePlayer"/> instance.<br/>
        ///     To get the online player instance, you should use the <see cref="IPlayerManager.GetOnlinePlayer"/> method.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        ///     Gets the last time the player was online on the server. Equals null if the player was never online.
        /// </summary>
        DateTime? LastSeen { get; }
    }
}