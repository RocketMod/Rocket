using System.Threading.Tasks;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.API.Player;
using Rocket.Core.User;

namespace Rocket.Core.Commands
{
    public class CommandContext : ICommandContext
    {
        public CommandContext(IDependencyContainer container,
                              IUser user,
                              string commandPrefix,
                              ICommand command,
                              string commandAlias,
                              string[] parameters,
                              ICommandContext parentCommandContext,
                              ICommandContext rootCommandContext)
        {
            Container = container;
            User = user;
            Command = command;
            CommandAlias = commandAlias;
            ParentContext = parentCommandContext;
            RootContext = rootCommandContext ?? this;
            Parameters = new CommandParameters(container, parameters);
            CommandPrefix = commandPrefix;
        }

        public ICommand Command { get; internal set; }
        public IUser User { get; }
        public ICommandContext ChildContext { get; internal set; }
        public ICommandContext ParentContext { get; }
        public ICommandContext RootContext { get; }
        public string CommandPrefix { get; }
        public string CommandAlias { get; }
        public ICommandParameters Parameters { get; }
        public IDependencyContainer Container { get; }

        public async Task SendCommandUsageAsync()
        {
            await User.SendMessageAsync("Usage: " + CommandPrefix + CommandAlias + " " + Command.Syntax, Color.Blue);
        }
    }
}