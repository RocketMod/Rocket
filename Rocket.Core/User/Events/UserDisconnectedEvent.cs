using Rocket.API.Eventing;
using Rocket.API.User;
using Rocket.Core.Eventing;

namespace Rocket.Core.User.Events
{
    public class UserDisconnectedEvent : Event
    {
        public UserDisconnectedEvent(IUserInfo user) : base(true)
        {
            User = user;
            Reason = null;
        }

        public UserDisconnectedEvent(IUserInfo user, string reason = null) : base(true)
        {
            User = user;
            Reason = reason;
        }

        public UserDisconnectedEvent(IUserInfo user, string reason = null, bool global = true) : base(global)
        {
            User = user;
            Reason = reason;
        }

        public UserDisconnectedEvent(IUserInfo user, string reason = null,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(executionTarget, global)
        {
            User = user;
            Reason = reason;
        }

        public IUserInfo User { get; }

        public string Reason { get; }
    }
}