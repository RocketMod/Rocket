using Rocket.API.User;

namespace Rocket.API.Entities
{
    /// <summary>
    ///     Represents a living entity with health.
    /// </summary>
    public interface ILivingEntity: IEntity
    {
        /// <summary>
        ///     The max health of the entity.
        /// </summary>
        double MaxHealth { get; }

        /// <summary>
        ///     The current health of the entitiy.
        /// </summary>
        double Health { get; set; }

        /// <summary>
        ///     Kills the entity.
        /// </summary>
        void Kill();

        /// <summary>
        ///     <inheritdoc cref="Kill()" />
        /// </summary>
        /// <param name="killer">the killer of the entity.</param>
        void Kill(IEntity killer);

        /// <summary>
        ///     <inheritdoc cref="Kill()" />
        /// </summary>
        /// <param name="killer">the killer of the entity.</param>
        void Kill(IUser killer);
    }
}