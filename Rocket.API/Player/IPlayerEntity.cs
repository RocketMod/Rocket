using System.Numerics;
using System.Threading.Tasks;
using Rocket.API.Entities;

namespace Rocket.API.Player
{
    /// <summary>
    ///     Represents a player entity.
    /// </summary>
    public interface IPlayerEntity<TPlayer> : IEntity where TPlayer : IPlayer
    {
        TPlayer Player { get; }
    }
}