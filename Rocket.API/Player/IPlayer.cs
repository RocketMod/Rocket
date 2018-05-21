using System;
using Rocket.API.Entities;
using Rocket.API.User;

namespace Rocket.API.Player
{
    /// <summary>
    ///     <inheritdoc cref="IPlayer" /><br /><br />
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the player.</typeparam>
    /// <typeparam name="TUser">The user type of the player.</typeparam>
    /// <typeparam name="TSelf">The player itself.</typeparam>
    public interface IPlayer<out TEntity, out TUser, out TSelf>: IPlayer 
        where TEntity: IPlayerEntity<TSelf>
        where TUser: IPlayerUser<TSelf>
        where TSelf : IPlayer
    {
        /// <summary>
        ///     The User that represents the online player. Might be null if not online.
        /// </summary>
        TUser User { get; }

        /// <summary>
        ///     The game object that is linked to this player.
        /// </summary>
        TEntity Entity { get; }
    }

    /// <summary>
    ///     This interface represents a player.
    /// </summary>
    public interface IPlayer : IIdentity, IFormattable
    {
        /// <summary>
        ///     Checks if the player is online.
        /// </summary>
        bool IsOnline { get; }
    }
}