using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.User;
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
        public string Syntax => BaseCommand.Syntax;
        public IChildCommand[] ChildCommands => BaseCommand.ChildCommands;
        public bool  SupportsUser(IUser user) => BaseCommand.SupportsUser(user);
        public async Task ExecuteAsync(ICommandContext context) => await BaseCommand.ExecuteAsync(context);
    }
}