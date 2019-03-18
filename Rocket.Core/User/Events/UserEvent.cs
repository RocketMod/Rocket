using Rocket.API.Eventing;
using Rocket.API.User;
using Rocket.Core.Eventing;

namespace Rocket.Core.User.Events
{
    public class UserEvent : Event
    {
        protected UserEvent(IUser user) : this(user, true) { }

        /// <param name="global">Defines if the event is emitted globally</param>
        protected UserEvent(IUser user, bool global = true) : base(global)
        {
            User = user;
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="EventExecutionTargetContext" /></param>
        /// <param name="global">Defines if the event is emitted globally</param>
        protected UserEvent(IUser user,
                            EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                            bool global = true) : base(executionTarget, global)
        {
            User = user;
        }

        public IUser User { get; }
    }
}