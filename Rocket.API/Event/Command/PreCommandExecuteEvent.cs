using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.API.Event.Command
{
    public class PreCommandExecuteEvent : ExecuteCommandEvent
    {
        public PreCommandExecuteEvent(IRocketPlayer player, IRocketCommand command, string[] args) : base(player, command, args)
        {

        }
    }
}