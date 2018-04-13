using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerBanEvent : PlayerEvent, ICancellableEvent
    {
        public IPlayer Banner { get; }
        public string Reason { get; set; }

        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null) : base(player)
        {
            Banner = banner;
            Reason = reason;
        }
        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null, bool global = true) : base(player, global)
        {
            Banner = banner;
            Reason = reason;
        }
        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Banner = banner;
            Reason = reason;
        }
        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Banner = banner;
            Reason = reason;
        }

        public bool IsCancelled { get; set; }
    }
}