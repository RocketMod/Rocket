using Rocket.API.Providers.Implementation;

namespace Rocket.API.Event.Implementation
{
    public abstract class ImplementationEvent : Event
    {
        public IGameProvider Implementation { get; }

        protected ImplementationEvent(IGameProvider implementation)
        {
            Implementation = implementation;
        }
    }
}