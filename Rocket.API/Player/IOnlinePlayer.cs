using System;
using Rocket.API.Commands;
using Rocket.API.Entities;

namespace Rocket.API.Player
{
    /// <summary>
    ///     <inheritdoc cref="IPlayer"/><br/><br/>
    ///     This interface represents an online player.
    /// </summary>
    public interface IOnlinePlayer : IPlayer, ICommandCaller, IEntity
    {
        /// <summary>
        ///     The player connect time.
        /// </summary>
        DateTime SessionConnectTime { get; }

        /// <summary>
        ///     The player disconnect time.
        /// </summary>
        DateTime? SessionDisconnectTime { get; }

        /// <summary>
        ///     The player online time.
        /// </summary>
        TimeSpan SessionOnlineTime { get; }
    }
}