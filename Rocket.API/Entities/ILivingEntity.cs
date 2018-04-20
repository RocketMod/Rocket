using Rocket.API.Commands;
using Rocket.API.Entities;

namespace Rocket.API.Player
{
    public interface ILivingEntity
    {
        double MaxHealth { get; set; }

        double Health { get; set; }

        void Kill();

        void Kill(IEntity killer);

        void Kill(ICommandCaller caller);
    }
}