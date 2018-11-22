using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerConnectedEvent : PlayerEvent
    {
        public PlayerConnectedEvent(IPlayer player) : base(player) { }
        public PlayerConnectedEvent(IPlayer player, bool global = true) : base(player, global) { }

        public PlayerConnectedEvent(IPlayer player,
                                  EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                  bool global = true) : base(player, executionTarget, global) { }
    }
}