using Rocket.API.Eventing;
using Rocket.API.User;
using Rocket.Core.Player.Events;

namespace Rocket.Core.User.Events
{
    public class UserChatEvent : UserEvent, ICancellableEvent
    {
        public UserChatEvent(IUser user, string message) : base(user)
        {
            Message = message;
        }

        public UserChatEvent(IUser user, string message, bool global = true) : base(user, global)
        {
            Message = message;
        }

        public UserChatEvent(IUser user, string message,
                             EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                             bool global = true) : base(user, executionTarget, global)
        {
            Message = message;
        }

        public string Message { get; set; }

        public bool IsCancelled { get; set; }
    }
}