using Rocket.API.Player;

namespace Rocket.API.Event.Player
{
    public class PrePlayerConnectedEvent : PlayerEvent
    {
        public PrePlayerConnectedEvent(IPlayer player) : base(player)
        {
        }
    }
}