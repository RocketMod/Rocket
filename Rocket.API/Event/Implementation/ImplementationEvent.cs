using Rocket.API.Providers.Implementation;

namespace Rocket.API.Event.Implementation
{
    public abstract class ImplementationEvent : Event
    {
        public IRocketImplementationProvider Implementation { get; }

        protected ImplementationEvent(IRocketImplementationProvider implementation)
        {
            Implementation = implementation;
        }
    }
}