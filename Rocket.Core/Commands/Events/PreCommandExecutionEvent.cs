using Rocket.API.Commands;
using Rocket.API.Eventing;

namespace Rocket.Core.Commands.Events
{
    public class PreCommandExecutionEvent : Event, ICancellableEvent
    {
        public PreCommandExecutionEvent(ICommandCaller player, string commandLine) :
            this(player, commandLine, false) { }

        public PreCommandExecutionEvent(ICommandCaller player, string commandLine, bool global = true) : base(global)
        {
            Player = player;
            CommandLine = commandLine;
        }

        public PreCommandExecutionEvent(ICommandCaller player, string commandLine,
                                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                        bool global = true) : base(executionTarget, global)
        {
            Player = player;
            CommandLine = commandLine;
        }

        public PreCommandExecutionEvent(ICommandCaller player, string commandLine, string name = null,
                                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                        bool global = true) : base(name, executionTarget, global)
        {
            Player = player;
            CommandLine = commandLine;
        }

        public ICommandCaller Player { get; }
        public string CommandLine { get; }

        public bool IsCancelled { get; set; }
    }
}