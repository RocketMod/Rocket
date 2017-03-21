using Rocket.API.Player;

namespace Rocket.API.Event.Player
{
    public class PlayerDisconnectedEvent : PlayerEvent
    {
        public PlayerDisconnectedEvent(IRocketPlayer player) : base(player)
        {
        }
    }
}