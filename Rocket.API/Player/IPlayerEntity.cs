using System.Numerics;
using System.Threading.Tasks;
using Rocket.API.Entities;

namespace Rocket.API.Player
{
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
        Task<bool> TeleportAsync(Vector3 position, float rotation);
    }
}