using System;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Player
{
    /// <summary>
    ///     This interface represents a player.
    /// </summary>
    public interface IPlayer<TUser, TEntity, TSelf> : IPlayer
        where TUser : IPlayerUser<TSelf>
        where TEntity : IPlayerEntity<TSelf>
        where TSelf : IPlayer
    {
        TUser User { get; }

        TEntity Entity { get; }
    }

    public interface IPlayer : IFormattable
    {
        /// <summary>
        ///     Checks if the player is online.
        /// </summary>
        bool IsOnline { get; }

        IDependencyContainer Container { get; }

        /// <summary>
        ///     The User connect time.
        /// </summary>
        DateTime SessionConnectTime { get; }

        /// <summary>
        ///     The User disconnect time.
        /// </summary>
        DateTime? SessionDisconnectTime { get; }

        /// <summary>
        ///     The PlayerManager of the player.
        /// </summary>
        IPlayerManager PlayerManager { get; }
    }
}