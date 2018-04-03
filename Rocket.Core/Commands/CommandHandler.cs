using System;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.Core.Extensions;

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
            var commands = container.GetAll<ICommandProvider>().SelectMany(c => c.Commands);
            var args = commandLine.Split(' ');

            ICommand target = commands.FirstOrDefault(c => c.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));
            if (target == null)
                return false; // only return false when the command was not found

            var context = new CommandContext(caller, args[0], args.Skip(1).ToArray());

            try
            {
                target.Execute(context);
            }
            catch (Exception e)
            {
                foreach (var handler in container.GetHandlers<ICommandExceptionHandler>())
                {
                    if (handler.HandleException(context, e))
                    {
                        return true;
                    }
                }

                throw;
            }

            return true;
        }
    }
}