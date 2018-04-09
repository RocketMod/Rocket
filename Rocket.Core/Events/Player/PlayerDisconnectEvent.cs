using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerDisconnectEvent : PlayerEvent
    {
        public string Reason { get; }

        public PlayerDisconnectEvent(IPlayer player, string reason) : base(player)
        {
            Reason = reason;
        }

        public PlayerDisconnectEvent(IPlayer player, string reason, bool global = true) : base(player, global)
        {
            Reason = reason;
        }

        public PlayerDisconnectEvent(IPlayer player, string reason,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(player, executionTarget, global)
        {
            Reason = reason;
        }

        public PlayerDisconnectEvent(IPlayer player, string reason, string name = null,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(player, name, executionTarget, global)
        {
            Reason = reason;
        }
    }
}