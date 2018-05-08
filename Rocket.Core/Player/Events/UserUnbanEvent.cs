using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Core.Player.Events
{
    public class UserUnbanEvent: Event, ICancellableEvent
    {
        public UserUnbanEvent(IUserInfo user, IUser caller = null) : base(true)
        {
            User = user;
            Caller = caller;
        }

        public UserUnbanEvent(IUserInfo user, IUser caller = null, bool global = true) : base(global)
        {
            User = user;
            Caller = caller;
        }

        public UserUnbanEvent(IUserInfo user, IUser caller = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            User = user;
            Caller = caller;
        }

        public UserUnbanEvent(IUserInfo user, IUser caller = null, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            User = user;
            Caller = caller;
        }

        public IUserInfo User { get; }
        public IUser Caller { get; }

        public bool IsCancelled { get; set; }

    }
}