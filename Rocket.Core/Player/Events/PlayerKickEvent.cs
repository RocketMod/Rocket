using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerKickEvent : OnlinePlayerEvent, ICancellableEvent
    {
        public PlayerKickEvent(IOnlinePlayer player, ICommandCaller caller = null, string reason = null) : base(player)
        {
            Caller = caller;
            Reason = reason;
        }

        public PlayerKickEvent(IOnlinePlayer player, ICommandCaller caller = null, string reason = null,
                               bool global = true) : base(player,
            global)
        {
            Caller = caller;
            Reason = reason;
        }

        public PlayerKickEvent(IOnlinePlayer player, ICommandCaller caller = null, string reason = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, executionTarget, global)
        {
            Caller = caller;
            Reason = reason;
        }

        public PlayerKickEvent(IOnlinePlayer player, ICommandCaller caller = null, string reason = null,
                               string name = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, name, executionTarget, global)
        {
            Caller = caller;
            Reason = reason;
        }

        public ICommandCaller Caller { get; }
        public string Reason { get; set; }

        public bool IsCancelled { get; set; }
    }
}