using Rocket.API.Player;

namespace Rocket.API.Event.Player
{
    public abstract class PlayerEvent : Event
    {
        public IRocketPlayer Player { get; }

        protected PlayerEvent(IRocketPlayer player)
        {
            Player = player;
        }
    }
}