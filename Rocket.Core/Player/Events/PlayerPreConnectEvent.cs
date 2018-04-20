using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerPreConnectEvent : PlayerEvent, ICancellableEvent
    {
        public virtual string RejectionReason { get; set; }

        public PlayerPreConnectEvent(IOnlinePlayer player) : base(player, true) { }
        public PlayerPreConnectEvent(IOnlinePlayer player, bool global = true) : base(player, global) { }

        public PlayerPreConnectEvent(IOnlinePlayer player,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(player, executionTarget, global) { }

        public PlayerPreConnectEvent(IOnlinePlayer player, string name = null,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(player, name, executionTarget, global) { }

        public bool IsCancelled { get; set; }
    }
}