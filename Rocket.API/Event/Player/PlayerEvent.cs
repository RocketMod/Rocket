using Rocket.API.Player;

namespace Rocket.API.Event.Player
{
    public abstract class PlayerEvent : Event
    {
        public IPlayer Player { get; }

        protected PlayerEvent(IPlayer player)
        {
            Player = player;
        }
    }
}