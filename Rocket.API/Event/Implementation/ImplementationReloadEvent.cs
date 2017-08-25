using Rocket.API.Providers.Implementation;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationReloadEvent : ImplementationEvent
    {
        public ImplementationReloadEvent(IGameProvider implementation) : base(implementation)
        {
        }
    }
}