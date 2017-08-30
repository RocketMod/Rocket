using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Core.Commands
{
    public class CommandContext: ICommandContext
    {
        public CommandContext(IPlayer caller, string[] arguments, ICommand command, ICommandOutput output)
        {
            Caller = caller;
            Parameters = arguments;
            Command = command;
            Output = output;
        }

        public IPlayer Caller { get; }
        public string[] Parameters { get; }
        public ICommand Command { get; }
        public ICommandOutput Output { get; }
    }
}