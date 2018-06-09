using Rocket.API.Eventing;
using Rocket.API.User;

namespace Rocket.Core.User.Events
{
    public class UserConnectedEvent : UserEvent
    {
        public UserConnectedEvent(IUser user) : base(user) { }
        public UserConnectedEvent(IUser user, bool global = true) : base(user, global) { }

        public UserConnectedEvent(IUser user,
                                  EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                  bool global = true) : base(user, executionTarget, global) { }
    }
}