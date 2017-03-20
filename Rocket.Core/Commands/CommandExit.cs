using Rocket.API;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Core.Commands
{
    public class CommandExit : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Name
        {
            get { return "exit"; }
        }

        public string Help
        {
            get { return "Exit the game";}
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.exit" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            caller.Kick("You exited.");
        }
    }
}