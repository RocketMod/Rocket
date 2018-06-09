using Rocket.API.Eventing;
using Rocket.API.User;
using Rocket.Core.Eventing;

namespace Rocket.Core.Commands.Events
{
    public class PreCommandExecutionEvent : Event, ICancellableEvent
    {
        public PreCommandExecutionEvent(IUser user, string commandLine) :
            this(user, commandLine, false) { }

        public PreCommandExecutionEvent(IUser user, string commandLine, bool global = true) : base(global)
        {
            User = user;
            CommandLine = commandLine;
        }

        public PreCommandExecutionEvent(IUser user, string commandLine,
                                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                        bool global = true) : base(executionTarget, global)
        {
            User = user;
            CommandLine = commandLine;
        }

        public IUser User { get; }
        public string CommandLine { get; }

        public bool IsCancelled { get; set; }
    }
}