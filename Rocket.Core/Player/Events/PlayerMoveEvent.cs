using System.Numerics;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerMoveEvent : PlayerEvent
    {
        public PlayerMoveEvent(Vector3 oldPosition, Vector3 newPosition, IPlayer player) : base(player)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public PlayerMoveEvent(Vector3 oldPosition, Vector3 newPosition, IPlayer player, bool global = true) :
            base(player, global)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public PlayerMoveEvent(Vector3 oldPosition, Vector3 newPosition, IPlayer player,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(player, executionTarget, global)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public Vector3 OldPosition { get; }
        public Vector3 NewPosition { get; }
    }
}