using Rocket.API.Player;

namespace Rocket.API.Event.Player
{
    public class PlayerDisconnectedEvent : PlayerEvent
    {
        public PlayerDisconnectedEvent(IPlayer player) : base(player)
        {
        }
    }
}