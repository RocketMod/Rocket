using Rocket.API.Player;

namespace Rocket.API.Event.Player
{
    public class PlayerConnectedEvent : PlayerEvent
    {
        public PlayerConnectedEvent(IRocketPlayer player) : base(player)
        {
        }
    }
}