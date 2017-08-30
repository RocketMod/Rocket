using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.API.Event.Command
{
    public class PreCommandExecuteEvent : ExecuteCommandEvent
    {
        public PreCommandExecuteEvent(IPlayer player, ICommand command, string[] args) : base(player, command, args)
        {

        }
    }
}