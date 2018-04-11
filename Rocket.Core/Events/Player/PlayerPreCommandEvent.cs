using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerPreCommandEvent : PlayerEvent, ICancellableEvent
    {
        public string CommandLine { get; }
        public PlayerPreCommandEvent(IPlayer player, string commandLine) : base(player)
        {
            CommandLine = commandLine;
        }
        public PlayerPreCommandEvent(IPlayer player, string commandLine, bool global = true) : base(player, global)
        {
            CommandLine = commandLine;
        }
        public PlayerPreCommandEvent(IPlayer player, string commandLine, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            CommandLine = commandLine;
        }
        public PlayerPreCommandEvent(IPlayer player, string commandLine, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            CommandLine = commandLine;
        }

        public bool IsCancelled { get; set; }
    }
}