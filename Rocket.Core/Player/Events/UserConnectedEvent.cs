using Rocket.API.Eventing;
using Rocket.API.User;

namespace Rocket.Core.Player.Events
{
    public class UserConnectedEvent : UserEvent
    {
        public UserConnectedEvent(IUser user) : base(user) { }
        public UserConnectedEvent(IUser user, bool global = true) : base(user, global) { }

        public UserConnectedEvent(IUser user,
                                    EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                    bool global = true) : base(user, executionTarget, global) { }

        public UserConnectedEvent(IUser user, string name = null,
                                    EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                    bool global = true) : base(user, name, executionTarget, global) { }
    }
}