using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerConnectEvent : PlayerEvent
    {
        public PlayerConnectEvent(IPlayer player, string reason, bool global = true) : base(player, global)
        {

        }
        public PlayerConnectEvent(IPlayer player, string reason, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
        }
        public PlayerConnectEvent(IPlayer player, string reason, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
        }
    }
}