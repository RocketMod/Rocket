using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerPreConnectEvent : PlayerEvent, ICancellableEvent
    {
        public virtual string RejectReason { get; set; }

        public PlayerPreConnectEvent(IPlayer player, bool global = true) : base(player, global) { }
        public PlayerPreConnectEvent(IPlayer player, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global) { }
        public PlayerPreConnectEvent(IPlayer player, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global) { }
        public bool IsCancelled { get; set; }
    }
}