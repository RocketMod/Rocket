using Rocket.API.Eventing;
using Rocket.API.User;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerKickEvent : PlayerEvent, ICancellableEvent
    {
        public PlayerKickEvent(IPlayer player, IUser kickedBy = null, string reason = null) : base(player)
        {
            KickedBy = kickedBy;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, IUser kickedBy = null, string reason = null,
                             bool global = true) : base(player,
            global)
        {
            KickedBy = kickedBy;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, IUser kickedBy = null, string reason = null,
                             EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                             bool global = true) : base(player, executionTarget, global)
        {
            KickedBy = kickedBy;
            Reason = reason;
        }

        public string Reason { get; set; }
        public IUser KickedBy { get; set; }
        public bool IsCancelled { get; set; }
    }
}