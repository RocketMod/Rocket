using Rocket.API.Providers;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationShutdownEvent : GameEvent
    {
        public ImplementationShutdownEvent(IGameProvider implementation) : base(implementation)
        {
        }
    }
}