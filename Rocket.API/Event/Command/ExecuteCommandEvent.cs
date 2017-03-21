using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.API.Event.Command
{
    public class ExecuteCommandEvent : Event, ICancellableEvent
    {
        public IRocketPlayer Player { get; }
        public IRocketCommand Command { get; }
        public string[] Arguments { get; }

        public ExecuteCommandEvent(IRocketPlayer player, IRocketCommand command, string[] args)
        {
            Player = player;
            Command = command;
            Arguments = args;
        }

        public bool IsCancelled { get; set; }
    }
}