using Rocket.API.Commands;
using System;
using Rocket.API.Player;

namespace Rocket.API.Exceptions
{
    public class NoPermissionsForCommandException : Exception
    {
        private ICommand command;
        private IPlayer player;

        public NoPermissionsForCommandException(IPlayer player, ICommand command)
        {
            this.command = command;
            this.player = player;
        }

        public override string Message
        {
            get
            {
                return "The player " + player.DisplayName + " has no permission to execute the command " + command.Name;
            }
        }
    }
}