using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerUnbanEvent : UserUnbanEvent
    {
        public PlayerUnbanEvent(IUserInfo player, IUser unbannedBy = null) : base(player, unbannedBy)
        {
        }
        public PlayerUnbanEvent(IUserInfo player, IUser unbannedBy = null, bool global = true) : base(player, unbannedBy, global)
        {
        }
        public PlayerUnbanEvent(IUserInfo player, IUser unbannedBy = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, unbannedBy, executionTarget, global)
        {
        }
        public PlayerUnbanEvent(IUserInfo player, IUser unbannedBy = null, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, unbannedBy, name, executionTarget, global)
        {
        }
    }
}