using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Providers;

namespace Rocket.Core.Commands
{
    public class CommandExit : ICommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "exit";

        public string Help => "Exit the game";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.exit" };

        IPlayerProvider Players;
        public CommandExit(IPlayerProvider playerProvider)
        {
            Players = playerProvider;
        }

        public void Execute(ICommandContext ctx)
        {
            Players.Kick(ctx.Caller,"You exited.");
        }
    }
}