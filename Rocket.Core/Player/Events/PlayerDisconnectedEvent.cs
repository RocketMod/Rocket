using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player.Events;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerDisconnectedEvent: UserDisconnectedEvent
    {
        public IPlayer Player { get; }
        public PlayerDisconnectedEvent(IPlayer player) : base(player.GetUser())
        {
            Player = player;
        }
        public PlayerDisconnectedEvent(IPlayer player, string reason = null) : base(player.GetUser(), reason)
        {
            Player = player;
        }
        public PlayerDisconnectedEvent(IPlayer player, string reason = null, bool global = true) : base(player.GetUser(), reason, global)
        {
            Player = player;
        }
        public PlayerDisconnectedEvent(IPlayer player, string reason = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.GetUser(), reason, executionTarget, global)
        {
            Player = player;
        }
    }
}