using System;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player.Events;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerBanEvent : UserBanEvent
    {

        public PlayerBanEvent(IUserInfo player, IUser bannedBy = null, string reason = null, TimeSpan? duration = null) : base(player, bannedBy, reason, duration)
        {
        }
        public PlayerBanEvent(IUserInfo player, IUser bannedBy = null, string reason = null, TimeSpan? duration = null, bool global = true) : base(player, bannedBy, reason, duration, global)
        {
        }
        public PlayerBanEvent(IUserInfo player, IUser bannedBy = null, string reason = null, TimeSpan? duration = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, bannedBy, reason, duration, executionTarget, global)
        {
        }
    }
}