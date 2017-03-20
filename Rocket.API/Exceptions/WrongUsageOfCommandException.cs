using Rocket.API.Commands;
using Rocket.API.Plugins;
using System;
using Rocket.API.Player;

namespace Rocket.API.Exceptions
{
    public class WrongUsageOfCommandException : Exception
    {
        private IRocketCommand command;
        private IRocketPlayer player;

        public WrongUsageOfCommandException(IRocketPlayer player, IRocketCommand command)
        {
            this.command = command;
            this.player = player;
        }

        public override string Message
        {
            get
            {
                return "The player " + player.DisplayName + " did not correctly use the command " + command.Name;
            }
        }
    }
}