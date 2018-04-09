using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerKickEvent : PlayerEvent
    {
        public ICommandCaller Kicker { get; }
        public virtual string Reason { get; set; }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker, string reason) : base(player)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker, string reason, bool global = true) : base(player,
            global)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker, string reason,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, executionTarget, global)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker, string reason, string name = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, name, executionTarget, global)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, string reason, bool global = true) : base(player, global)
        {
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, string reason,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, executionTarget, global)
        {
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, string reason, string name = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, name, executionTarget, global)
        {
            Reason = reason;
        }
    }
}