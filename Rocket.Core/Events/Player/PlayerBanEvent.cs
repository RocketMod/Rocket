using System;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerBanEvent : PlayerEvent, ICancellableEvent
    {
        public TimeSpan? Duration { get; set; }
        public IPlayer Banner { get; }
        public string Reason { get; set; }

        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null, TimeSpan? duration = null) : base(player)
        {
            Banner = banner;
            Reason = reason;
            Duration = duration;
        }
        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null, TimeSpan? duration = null, bool global = true) : base(player, global)
        {
            Banner = banner;
            Reason = reason;
            Duration = duration;
        }
        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null, TimeSpan? duration = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Banner = banner;
            Reason = reason;
            Duration = duration;
        }
        public PlayerBanEvent(IPlayer player, IPlayer banner = null, string reason = null, TimeSpan? duration = null, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Banner = banner;
            Reason = reason;
            Duration = duration;
        }

        public bool IsCancelled { get; set; }
    }
}