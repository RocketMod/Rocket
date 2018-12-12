using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerDisconnectedEvent : PlayerEvent
    {
        public PlayerDisconnectedEvent(IPlayer player) : base(player,true)
        {
            Reason = null;
        }

        public PlayerDisconnectedEvent(IPlayer player, string reason = null) : base(player, true)
        {
            Reason = reason;
        }

        public PlayerDisconnectedEvent(IPlayer player, string reason = null, bool global = true) : base(player, global)
        {
            Reason = reason;
        }

        public PlayerDisconnectedEvent(IPlayer player, string reason = null,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(player, executionTarget, global)
        {
            Reason = reason;
        }
        
        public string Reason { get; }
    }
}