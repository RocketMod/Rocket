using Rocket.API.Eventing;
using Rocket.API.User;

namespace Rocket.Core.Player.Events
{
    public class UserUnbanEvent : Event, ICancellableEvent
    {
        public UserUnbanEvent(IUserInfo user, IUser unbannedBy = null) : base(true)
        {
            User = user;
            UnbannedBy = unbannedBy;
        }

        public UserUnbanEvent(IUserInfo user, IUser unbannedBy = null, bool global = true) : base(global)
        {
            User = user;
            UnbannedBy = unbannedBy;
        }

        public UserUnbanEvent(IUserInfo user, IUser unbannedBy = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            User = user;
            UnbannedBy = unbannedBy;
        }

        public UserUnbanEvent(IUserInfo user, IUser unbannedBy = null, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            User = user;
            UnbannedBy = unbannedBy;
        }

        public IUserInfo User { get; }
        public IUser UnbannedBy { get; }

        public bool IsCancelled { get; set; }
    }
}