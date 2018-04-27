using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerRespawnEvent : OnlinePlayerEvent
    {
        public PlayerRespawnEvent(IOnlinePlayer player) : base(player) { }
        public PlayerRespawnEvent(IOnlinePlayer player, bool global = true) : base(player, global) { }
        public PlayerRespawnEvent(IOnlinePlayer player, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global) { }
        public PlayerRespawnEvent(IOnlinePlayer player, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global) { }
    }
}