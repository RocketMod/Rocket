using Rocket.API.Providers;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationInitializedEvent : ImplementationEvent
    {
        public ImplementationInitializedEvent(IRocketImplementationProvider implementation) : base(implementation)
        {
        }
    }
}