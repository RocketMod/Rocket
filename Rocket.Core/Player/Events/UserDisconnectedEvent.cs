using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Core.Player.Events
{
    public class UserDisconnectedEvent : Event
    {
        public IUserInfo User { get; }

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

        public UserDisconnectedEvent(IUserInfo user, string reason = null, string name = null,
                                       EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                       bool global = true) : base(name, executionTarget, global)
        {
            User = user;
            Reason = reason;
        }

        public string Reason { get; }
    }
}