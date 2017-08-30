using Rocket.API.Player;

namespace Rocket.API.Event.Player
{
    public class PlayerConnectedEvent : PlayerEvent
    {
        public PlayerConnectedEvent(IPlayer player) : base(player)
        {
        }
    }
}