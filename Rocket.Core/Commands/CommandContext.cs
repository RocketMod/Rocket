using System;
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
                              ICommandContext parentCommandContext, 
                              ICommandContext rootCommandContext)
        {
            Container = container;
            Caller = caller;
            Command = command;
            CommandAlias = commandAlias;
            ParentCommandContext = parentCommandContext;
            RootCommandContext = rootCommandContext ?? this;
            Parameters = new CommandParameters(container, parameters);
            CommandPrefix = commandPrefix;
        }

        public ICommand Command { get; internal set; }
        public ICommandCaller Caller { get; }
        public ICommandContext ChildContext { get; internal set; }
        public ICommandContext ParentCommandContext { get; }
        public ICommandContext RootCommandContext { get; }
        public string CommandPrefix { get; }
        public string CommandAlias { get; }
        public ICommandParameters Parameters { get; }
        public IDependencyContainer Container { get; }
        public void SendUsage()
        {
            Caller.SendMessage("Usage: " + CommandPrefix + CommandAlias + " " + Command.Syntax, ConsoleColor.Blue);
        }
    }
}