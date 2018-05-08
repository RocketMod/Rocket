using System;
using Rocket.API.Commands;
using Rocket.API.Entities;
using Rocket.API.User;

namespace Rocket.API.Player
{
    /// <summary>
    ///     <inheritdoc cref="IPlayer"/><br/><br/>
    ///     This interface represents an online player.
    /// </summary>
    public interface IPlayer : IIdentity
    {
        /// <summary>
        ///     The User that represents the online player. Might be null if not online.
        /// </summary>
        IUser User { get; }

        /// <summary>
        ///     The game object that is linked to this player.
        /// </summary>
        IEntity Entity { get; }

        /// <summary>
        ///     Checks if the player is online.
        /// </summary>
        bool IsOnline { get; }
    }
}