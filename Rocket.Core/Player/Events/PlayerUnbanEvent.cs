using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerUnbanEvent : UserUnbanEvent
    {
        public IPlayer Player { get; }
        public PlayerUnbanEvent(IPlayer player, IUser unbannedBy = null) : base(player.User, unbannedBy)
        {
            Player = player;
        }
        public PlayerUnbanEvent(IPlayer player, IUser unbannedBy = null, bool global = true) : base(player.User, unbannedBy, global)
        {
            Player = player;
        }
        public PlayerUnbanEvent(IPlayer player, IUser unbannedBy = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, unbannedBy, executionTarget, global)
        {
            Player = player;
        }
        public PlayerUnbanEvent(IPlayer player, IUser unbannedBy = null, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, unbannedBy, name, executionTarget, global)
        {
            Player = player;
        }
    }
}