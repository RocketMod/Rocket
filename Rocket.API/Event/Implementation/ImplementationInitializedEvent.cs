using Rocket.API.Providers;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationInitializedEvent : GameEvent
    {
        public ImplementationInitializedEvent(IGameProvider implementation) : base(implementation)
        {
        }
    }
}