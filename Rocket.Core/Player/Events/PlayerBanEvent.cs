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
        public IPlayer Player { get; }
        public PlayerBanEvent(IPlayer player, IUser bannedBy = null, string reason = null, TimeSpan? duration = null) : base(player.User, bannedBy, reason, duration)
        {
            Player = player;
        }
        public PlayerBanEvent(IPlayer player, IUser bannedBy = null, string reason = null, TimeSpan? duration = null, bool global = true) : base(player.User, bannedBy, reason, duration, global)
        {
            Player = player;
        }
        public PlayerBanEvent(IPlayer player, IUser bannedBy = null, string reason = null, TimeSpan? duration = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, bannedBy, reason, duration, executionTarget, global)
        {
            Player = player;
        }
        public PlayerBanEvent(IPlayer player, IUser bannedBy = null, string reason = null, TimeSpan? duration = null, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, bannedBy, reason, duration, name, executionTarget, global)
        {
            Player = player;
        }
    }
}