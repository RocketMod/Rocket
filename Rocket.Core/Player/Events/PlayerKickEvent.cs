using System;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player.Events;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerKickEvent: UserKickEvent
    {
        public IPlayer Player { get; }
        public PlayerKickEvent(IPlayer player, IUser kickedBy = null, string reason = null) : base(player.User, kickedBy, reason)
        {
            Player = player;
        }
        public PlayerKickEvent(IPlayer player, IUser kickedBy = null, string reason = null, bool global = true) : base(player.User, kickedBy, reason, global)
        {
            Player = player;
        }
        public PlayerKickEvent(IPlayer player, IUser kickedBy = null, string reason = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, kickedBy, reason, executionTarget, global)
        {
            Player = player;
        }
        public PlayerKickEvent(IPlayer player, IUser kickedBy = null, string reason = null, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, kickedBy, reason, name, executionTarget, global)
        {
            Player = player;
        }
    }
}