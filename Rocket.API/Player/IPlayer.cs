using System;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.User;

namespace Rocket.API.Player
{
    /// <summary>
    ///     This interface represents a player.
    /// </summary>
    public interface IPlayer<TUser, TEntity> : IPlayer
        where TUser : IUser
        where TEntity : IPlayerEntity
    {
        /// <summary>
        ///     Checks if the player is online.
        /// </summary>
        bool IsOnline { get; }

        new TUser User { get; }

        new TEntity Entity { get; }

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

    public interface IPlayer : IFormattable
    {
        /// <summary>
        ///     Checks if the player is online.
        /// </summary>
        bool IsOnline { get; }

        IUser User { get; }

        IDependencyContainer Container { get; }

        IPlayerEntity Entity { get; }

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