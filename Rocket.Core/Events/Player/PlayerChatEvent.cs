using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerChatEvent : PlayerEvent, ICancellableEvent
    {
        public string Message { get; set; }

        public string ChatFormat { get; set; }

        public PlayerChatEvent(IPlayer player, string format, string message, bool global = true) : base(player, global)
        {
            Message = message;
            ChatFormat = format;
        }
        public PlayerChatEvent(IPlayer player, string format, string message, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Message = message;
            ChatFormat = format;
        }
        public PlayerChatEvent(IPlayer player, string format, string message, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Message = message;
            ChatFormat = format;
        }

        public bool IsCancelled { get; set; }
    }
}