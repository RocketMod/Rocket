using System;
using Rocket.API.Commands;
using Rocket.Core.DependencyInjection;

namespace Rocket.Core.Commands {
    [DontAutoRegister]
    public class ProxyCommand : ICommand
    {
        public ICommand BaseCommand { get; }
        private readonly string commandName;

        public ProxyCommand(ICommand baseCommand, string commandName)
        {
            this.commandName = commandName;
            BaseCommand = baseCommand;
        }

        public string Name => commandName ?? BaseCommand.Name;
        public string[] Aliases => BaseCommand.Aliases;
        public string Summary => BaseCommand.Summary;
        public string Description => BaseCommand.Description;
        public string Permission => BaseCommand.Permission;
        public string Syntax => BaseCommand.Syntax;
        public IChildCommand[] ChildCommands => BaseCommand.ChildCommands;
        public bool SupportsUser(Type user) => BaseCommand.SupportsUser(user);
        public void Execute(ICommandContext context) => BaseCommand.Execute(context);
    }
}