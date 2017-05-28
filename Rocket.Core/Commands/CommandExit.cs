using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Core.Commands
{
    public class CommandExit : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "exit";

        public string Help => "Exit the game";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.exit" };

        public void Execute(ICommandContext ctx)
        {
            ctx.Caller.Kick("You exited.");
        }
    }
}