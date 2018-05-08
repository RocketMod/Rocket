using Rocket.API.Eventing;
using Rocket.API.User;

namespace Rocket.Core.Player.Events
{
    public class UserEvent : Event
    {
        protected UserEvent(IUser user) : this(user, true) { }

        /// <param name="global">Defines if the event is emitted globally</param>
        protected UserEvent(IUser user, bool global = true) : base(global)
        {
            User = user;
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget" /></param>
        /// <param name="global">Defines if the event is emitted globally</param>
        protected UserEvent(IUser user,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            User = user;
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget" /></param>
        /// <param name="name">The name of the event. Will be auto set when null.</param>
        /// ///
        /// <param name="global">Defines if the event is emitted globally</param>
        protected UserEvent(IUser user, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            User = user;
        }

        public IUser User { get; }
    }
}