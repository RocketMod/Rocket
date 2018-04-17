using Rocket.API.Commands;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Commands
{
    public class CommandContext : ICommandContext
    {
        public CommandContext(IDependencyContainer container, 
                              ICommandCaller caller, 
                              string commandPrefix, 
                              ICommand command, 
                              string commandAlias, 
                              string[] parameters,
                              ICommandContext parentCommandContext)
        {
            Container = container;
            Caller = caller;
            Command = command;
            CommandAlias = commandAlias;
            ParentCommandContext = parentCommandContext;
            Parameters = new CommandParameters(parameters);
            CommandPrefix = commandPrefix;
        }

        public ICommand Command { get; internal set; }
        public ICommandCaller Caller { get; }
        public ICommandContext ParentCommandContext { get; }
        public string CommandPrefix { get; }
        public string CommandAlias { get; }
        public ICommandParameters Parameters { get; }
        public IDependencyContainer Container { get; }
    }
}