using Rocket.API.Eventing;
using Rocket.API.User;

namespace Rocket.Core.User.Events
{
    public class UserKickEvent : UserEvent, ICancellableEvent
    {
        public UserKickEvent(IUser user, IUser kickedBy = null, string reason = null) : base(user)
        {
            KickedBy = kickedBy;
            Reason = reason;
        }

        public UserKickEvent(IUser user, IUser kickedBy = null, string reason = null,
                             bool global = true) : base(user,
            global)
        {
            KickedBy = kickedBy;
            Reason = reason;
        }

        public UserKickEvent(IUser user, IUser kickedBy = null, string reason = null,
                             EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                             bool global = true) : base(user, executionTarget, global)
        {
            KickedBy = kickedBy;
            Reason = reason;
        }

        public IUser KickedBy { get; }

        public string Reason { get; set; }

        public bool IsCancelled { get; set; }
    }
}