using System;
using Rocket.API.Eventing;
using Rocket.API.User;
using Rocket.Core.Eventing;

namespace Rocket.Core.User.Events
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

        public TimeSpan? Duration { get; set; }
        public IUser BannedBy { get; }
        public IUserInfo User { get; }
        public string Reason { get; set; }

        public bool IsCancelled { get; set; }
    }
}