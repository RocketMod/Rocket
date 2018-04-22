using Rocket.API.Eventing;
using Rocket.API.Math;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerMoveEvent : PlayerEvent
    {
        public RVector3 OldPosition { get; }
        public RVector3 NewPosition { get; }

        public PlayerMoveEvent(RVector3 oldPosition, RVector3 newPosition, IOnlinePlayer player) : base(player)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
        public PlayerMoveEvent(RVector3 oldPosition, RVector3 newPosition, IOnlinePlayer player, bool global = true) : base(player, global)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
        public PlayerMoveEvent(RVector3 oldPosition, RVector3 newPosition, IOnlinePlayer player, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
        public PlayerMoveEvent(RVector3 oldPosition, RVector3 newPosition, IOnlinePlayer player, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}