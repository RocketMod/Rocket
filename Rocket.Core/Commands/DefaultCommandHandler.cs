using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.Core.Exceptions;
using Rocket.Core.Permissions;

namespace Rocket.Core.Commands
{
    public class DefaultCommandHandler : ICommandHandler
    {
        private readonly IDependencyContainer container;

        public DefaultCommandHandler(IDependencyContainer container)
        {
            this.container = container;
        }

        public bool HandleCommand(ICommandCaller caller, string commandLine, string prefix)
        {
            GuardCaller(caller);

            commandLine = commandLine.Trim();
            string[] args = commandLine.Split(' ');

            CommandContext context = new CommandContext(container.CreateChildContainer(), caller, prefix, null, args[0], args.Skip(1).ToArray());

            ICommand target = GetCommand(context);
            if (target == null)
                return false; // only return false when the command was not found

            context.Command = target;

            var tmp = new List<string> { target.Name };
            if (target.Permission != null)
                tmp.Add(target.Permission);

            var perms = tmp.ToArray();

            var provider = container.Get<IPermissionProvider>();
            if (provider.CheckHasAnyPermission(caller, perms) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(caller, perms);

            try
            {
                target.Execute(context);
            }
            catch (Exception e)
            {
                if (e is ICommandFriendlyException exception)
                {
                    exception.SendErrorMessage(context);
                    return true;
                }

                throw;
            }

            return true;
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }

        public ICommand GetCommand(ICommandContext ctx)
        {
            GuardCaller(ctx.Caller);

            IEnumerable<ICommand> commands = container.Get<ICommandProvider>().Commands;
            return commands
                   .Where(c => c.SupportsCaller(ctx.Caller))
                   .FirstOrDefault(c => c.Name.Equals(ctx.CommandAlias, StringComparison.OrdinalIgnoreCase));
        }

        private void GuardCaller(ICommandCaller caller)
        {
            if (!SupportsCaller(caller))
                throw new NotSupportedException(caller.GetType().FullName + " is not supported!");
        }
    }
}