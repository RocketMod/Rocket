using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerRespawnEvent : PlayerEvent
    {
        public PlayerRespawnEvent(IPlayer player) : base(player) { }
        public PlayerRespawnEvent(IPlayer player, bool global = true) : base(player, global) { }

        public PlayerRespawnEvent(IPlayer player,
                                  EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                  bool global = true) : base(player, executionTarget, global) { }
    }
}