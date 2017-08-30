using Rocket.API.Providers;

namespace Rocket.API.Event.Implementation
{
    public abstract class GameEvent : Event
    {
        public IGameProvider Implementation { get; }

        protected GameEvent(IGameProvider implementation)
        {
            Implementation = implementation;
        }
    }
}