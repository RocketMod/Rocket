using Rocket.API.Providers;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationReloadEvent : ImplementationEvent
    {
        public ImplementationReloadEvent(IRocketImplementationProvider implementation) : base(implementation)
        {
        }
    }
}