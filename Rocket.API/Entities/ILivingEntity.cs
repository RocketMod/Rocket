using Rocket.API.Entities;

namespace Rocket.API.Player
{
    public interface ILivingEntity
    {
        double MaxHealth { get; }

        double Health { get; }

        void Kill();

        void Kill(IEntity killer);
    }
}