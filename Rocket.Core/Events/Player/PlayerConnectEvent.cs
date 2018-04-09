using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerConnectEvent : PlayerEvent
    {
        public PlayerConnectEvent(IPlayer player) : base(player) { }
        public PlayerConnectEvent(IPlayer player, bool global = true) : base(player, global) { }

        public PlayerConnectEvent(IPlayer player,
                                  EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                  bool global = true) : base(player, executionTarget, global) { }

        public PlayerConnectEvent(IPlayer player, string name = null,
                                  EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                  bool global = true) : base(player, name, executionTarget, global) { }
    }
}