using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerChatEvent: UserChatEvent
    {
        public IPlayer Player { get; }
        public PlayerChatEvent(IPlayer player, string message) : base(player.Extend().User, message)
        {
            Player = player;
        }
        public PlayerChatEvent(IPlayer player, string message, bool global = true) : base(player.Extend().User, message, global)
        {
            Player = player;
        }
        public PlayerChatEvent(IPlayer player, string message, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.Extend().User, message, executionTarget, global)
        {
            Player = player;
        }
        public PlayerChatEvent(IPlayer player, string message, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.Extend().User, message, name, executionTarget, global)
        {
            Player = player;
        }
    }
}