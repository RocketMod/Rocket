using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerChatEvent : PlayerEvent, ICancellableEvent
    {
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

        public string Message { get; set; }

        public bool IsCancelled { get; set; }
    }
}