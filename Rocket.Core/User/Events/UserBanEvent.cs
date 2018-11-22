using System;
using Rocket.API.Eventing;
using Rocket.API.User;

namespace Rocket.Core.User.Events
{
    public class UserBanEvent : UserEvent, ICancellableEvent
    {
        public UserBanEvent(IUser user, IUser bannedBy = null, string reason = null,
                            TimeSpan? duration = null) : base(user,true)
        {
            BannedBy = bannedBy;
            Reason = reason;
            Duration = duration;
        }

        public UserBanEvent(IUser user, IUser bannedBy = null, string reason = null,
                            TimeSpan? duration = null, bool global = true) : base(user,global)
        {
            BannedBy = bannedBy;
            Reason = reason;
            Duration = duration;
        }

        public UserBanEvent(IUser user, IUser bannedBy = null, string reason = null,
                            TimeSpan? duration = null,
                            EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                            bool global = true) : base(user,executionTarget, global)
        {
            BannedBy = bannedBy;
            Reason = reason;
            Duration = duration;
        }

        public TimeSpan? Duration { get; set; }
        public IUser BannedBy { get; }
        public string Reason { get; set; }

        public bool IsCancelled { get; set; }
    }
}