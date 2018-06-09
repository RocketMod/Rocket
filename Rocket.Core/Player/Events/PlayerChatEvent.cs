using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerChatEvent: UserChatEvent
    {
        public IPlayer Player { get; }
        public PlayerChatEvent(IPlayer player, string message) : base(player.GetUser(), message)
        {
            Player = player;
        }
        public PlayerChatEvent(IPlayer player, string message, bool global = true) : base(player.GetUser(), message, global)
        {
            Player = player;
        }
        public PlayerChatEvent(IPlayer player, string message, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.GetUser(), message, executionTarget, global)
        {
            Player = player;
        }
    }
}