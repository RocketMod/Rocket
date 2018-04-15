using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player
{
    public class PlayerChatEvent : PlayerEvent, ICancellableEvent
    {
        public string Message { get; set; }

        public PlayerChatEvent(IPlayer player, string message) : base(player)
        {
            Message = message;
        }

        public PlayerChatEvent(IPlayer player, string message, bool global = true) : base(player, global)
        {
            Message = message;
        }

        public PlayerChatEvent(IPlayer player, string message,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, executionTarget, global)
        {
            Message = message;
        }

        public PlayerChatEvent(IPlayer player, string message, string name = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, name, executionTarget, global)
        {
            Message = message;
        }

        public bool IsCancelled { get; set; }
    }
}