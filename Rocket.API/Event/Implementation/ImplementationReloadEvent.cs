using Rocket.API.Providers;
using Rocket.API.Providers.Implementation;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationReloadEvent : ImplementationEvent
    {
        public ImplementationReloadEvent(IRocketImplementationProvider implementation) : base(implementation)
        {
        }
    }
}