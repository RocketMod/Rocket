using System;
using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Core.Player.Events
{
    public class UserBanEvent : Event, ICancellableEvent
    {
        public UserBanEvent(IUserInfo user, IUser bannedBy = null, string reason = null,
                              TimeSpan? duration = null) : base(true)
        {
            BannedBy = bannedBy;
            User = user;
            Reason = reason;
            Duration = duration;
        }

        public UserBanEvent(IUserInfo user, IUser bannedBy = null, string reason = null,
                              TimeSpan? duration = null, bool global = true) : base(global)
        {
            BannedBy = bannedBy;
            User = user;
            Reason = reason;
            Duration = duration;
        }

        public UserBanEvent(IUserInfo user, IUser bannedBy = null, string reason = null,
                              TimeSpan? duration = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            BannedBy = bannedBy;
            User = user;
            Reason = reason;
            Duration = duration;
        }

        public UserBanEvent(IUserInfo user, IUser bannedBy = null, string reason = null,
                              TimeSpan? duration = null, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            BannedBy = bannedBy;
            User = user;
            Reason = reason;
            Duration = duration;
        }

        public TimeSpan? Duration { get; set; }
        public IUser BannedBy { get; }
        public IUserInfo User { get; }
        public string Reason { get; set; }

        public bool IsCancelled { get; set; }
    }
}