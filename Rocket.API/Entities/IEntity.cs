using System.Numerics;
using System.Threading.Tasks;

namespace Rocket.API.Entities
{
    /// <summary>
    ///     A game entitiy. Players, Monsters, Animals, Vehicles, Zombies, etc...
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        ///     The type name of the entitiy. "Zombie", "Player", "Animal", etc...
        /// </summary>
        string EntityTypeName { get; }

        /// <summary>
        ///     The world position of the entity.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        ///     Teleports the player to the given position.
        /// </summary>
        /// <param name="position">The position to teleport to.</param>
        /// <param name="rotation">Rotation of the entity on the Y axis.</param>
        /// <returns><b>True</b> if the teleport was succesful, otherwise; <b>false</b>.</returns>
        Task<bool> TeleportAsync(Vector3 position, float rotation);
    }
}