using Rocket.API.Entities;

namespace Rocket.API.Player
{
    /// <summary>
    ///     Represents a player entity.
    /// </summary>
    public interface IPlayerEntity: IEntity
    {
        /// <summary>
        ///     The player instance.
        /// </summary>
        IPlayer Player { get; }
    }
}