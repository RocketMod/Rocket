using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerKickEvent : PlayerEvent, ICancellableEvent
    {
        public ICommandCaller Kicker { get; }
        public string Reason { get; set; }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker = null, string reason = null) : base(player)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker = null, string reason = null, bool global = true) : base(player,
            global)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker = null, string reason = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, executionTarget, global)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public PlayerKickEvent(IPlayer player, ICommandCaller kicker = null, string reason = null, string name = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, name, executionTarget, global)
        {
            Kicker = kicker;
            Reason = reason;
        }

        public bool IsCancelled { get; set; }
    }
}