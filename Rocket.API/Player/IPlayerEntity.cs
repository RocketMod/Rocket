using System.Numerics;
using Rocket.API.Entities;

namespace Rocket.API.Player
{
    /// <summary>
    ///     <inheritdoc cref="IPlayerEntity"/>
    /// </summary>
    public interface IPlayerEntity<out TPlayer>: IPlayerEntity where TPlayer: IPlayer
    {
        /// <summary>
        ///     The player instance.
        /// </summary>
        TPlayer Player { get; }
    }

    /// <summary>
    ///     Represents a player entity.
    /// </summary>
    public interface IPlayerEntity : IEntity
    {
        /// <summary>
        ///     Teleports the player to the given position.
        /// </summary>
        /// <param name="position">The position to teleport to.</param>
        /// <returns><b>True</b> if the teleport was succesful, otherwise; <b>false</b>.</returns>
        bool Teleport(Vector3 position);
    }
}