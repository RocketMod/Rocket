using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerConnectedEvent: UserConnectedEvent
    {
        public IPlayer Player { get; }
        public PlayerConnectedEvent(IPlayer player) : base(player.GetUser())
        {
            Player = player;
        }
        public PlayerConnectedEvent(IPlayer player, bool global = true) : base(player.GetUser(), global)
        {
            Player = player;
        }
        public PlayerConnectedEvent(IPlayer player, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.GetUser(), executionTarget, global)
        {
            Player = player;
        }
    }
}