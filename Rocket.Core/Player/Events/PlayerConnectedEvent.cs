using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerConnectedEvent : PlayerEvent
    {
        public PlayerConnectedEvent(IOnlinePlayer player) : base(player) { }
        public PlayerConnectedEvent(IOnlinePlayer player, bool global = true) : base(player, global) { }

        public PlayerConnectedEvent(IOnlinePlayer player,
                                  EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                  bool global = true) : base(player, executionTarget, global) { }

        public PlayerConnectedEvent(IOnlinePlayer player, string name = null,
                                  EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                  bool global = true) : base(player, name, executionTarget, global) { }
    }
}