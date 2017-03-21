using Rocket.API.Providers;

namespace Rocket.API.Event.Implementation
{
    public class ImplementationShutdownEvent : ImplementationEvent
    {
        public ImplementationShutdownEvent(IRocketImplementationProvider implementation) : base(implementation)
        {
        }
    }
}