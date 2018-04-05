using Rocket.API.Commands;

namespace Rocket.Core.Commands
{
    public class CommandContext : ICommandContext
    {
        public CommandContext(ICommandCaller caller, string command, string[] parameters)
        {
            Caller = caller;
            Command = command;
            Parameters = parameters;
        }

        public ICommandCaller Caller { get; }
        public string Command { get; }
        public string[] Parameters { get; }
    }
}