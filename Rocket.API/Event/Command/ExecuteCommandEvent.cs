using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.API.Event.Command
{
    public class ExecuteCommandEvent : Event, ICancellableEvent
    {
        public IPlayer Player { get; }
        public ICommand Command { get; }
        public string[] Arguments { get; }

        public ExecuteCommandEvent(IPlayer player, ICommand command, string[] args)
        {
            Player = player;
            Command = command;
            Arguments = args;
        }

        public bool IsCancelled { get; set; }
    }
}