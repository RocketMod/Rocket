using Rocket.API.Eventing;
using Rocket.API.User;

namespace Rocket.Core.User.Events
{
    public class UserUnbanEvent : UserEvent, ICancellableEvent
    {
        public UserUnbanEvent(IUser user, IUser unbannedBy = null) : base(user,true)
        {
            UnbannedBy = unbannedBy;
        }

        public UserUnbanEvent(IUser user, IUser unbannedBy = null, bool global = true) : base(user, global)
        {
            UnbannedBy = unbannedBy;
        }

        public UserUnbanEvent(IUser user, IUser unbannedBy = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(user, executionTarget, global)
        {
            UnbannedBy = unbannedBy;
        }

        public IUser UnbannedBy { get; }
        public bool IsCancelled { get; set; }
    }
}