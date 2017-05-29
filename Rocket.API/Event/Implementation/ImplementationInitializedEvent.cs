using Rocket.API.Providers.Implementation;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationInitializedEvent : ImplementationEvent
    {
        public ImplementationInitializedEvent(IRocketImplementationProvider implementation) : base(implementation)
        {
        }
    }
}