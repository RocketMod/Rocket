using System;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.Core.Exceptions;

namespace Rocket.Core.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IDependencyContainer container;

        public CommandHandler(IDependencyContainer container)
        {
            this.container = container;
        }

        public bool HandleCommand(ICommandCaller caller, string commandLine)
        {
            commandLine = commandLine.Trim();
            var args = commandLine.Split(' ');

            var context = new CommandContext(caller, args[0], args.Skip(1).ToArray());

            var target = GetCommand(context);
            if (target == null)
                return false; // only return false when the command was not found

            try
            {
                target.Execute(context);
            }
            catch (Exception e)
            {
                if (e is IFriendlyException)
                {
                    ((IFriendlyException) e).ToFriendlyString(context);
                    return true;
                }

                throw;
            }

            return true;
        }

        public ICommand GetCommand(ICommandContext ctx)
        {
            var commands = container.GetAll<ICommandProvider>().SelectMany(c => c.Commands);
            return commands.FirstOrDefault(c => c.Name.Equals(ctx.Command, StringComparison.OrdinalIgnoreCase));
        }
    }
}