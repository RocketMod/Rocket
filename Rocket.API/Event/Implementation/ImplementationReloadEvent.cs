using Rocket.API.Providers;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationReloadEvent : GameEvent
    {
        public ImplementationReloadEvent(IGameProvider implementation) : base(implementation)
        {
        }
    }
}