using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Ioc;
using Rocket.API.Permissions;
using Rocket.Core.Exceptions;

namespace Rocket.Core.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IDependencyContainer container;
        private readonly IPermissionProvider provider;
        
        public CommandHandler(IDependencyContainer container, IPermissionProvider provider)
        {
            this.container = container;
            this.provider = provider;
        }

        public bool HandleCommand(ICommandCaller caller, string commandLine)
        {
            commandLine = commandLine.Trim();
            string[] args = commandLine.Split(' ');

            CommandContext context = new CommandContext(caller, args[0], args.Skip(1).ToArray());

            ICommand target = GetCommand(context);
            if (target == null)
                return false; // only return false when the command was not found

            var perms = new[] {target.Permission, target.Name};

            if (!provider.HasAnyPermissions(caller, perms))
                throw new NotEnoughPermissionsException(caller, perms);

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
            IEnumerable<ICommand> commands = container.GetAll<ICommandProvider>().SelectMany(c => c.Commands);
            return commands.FirstOrDefault(c => c.Name.Equals(ctx.Command, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class NotEnoughPermissionsException : Exception
    {
        public ICommandCaller Caller { get; }
        public string[] Permissions { get; }

        public NotEnoughPermissionsException(ICommandCaller caller, string[] permissions)
        {
            Caller = caller;
            Permissions = permissions;
        }

        public override string Message
        {
            get
            {
                string message = $"{Caller.Name} does not have the following permissions: ";
                message += Environment.NewLine;
                foreach (var perm in Permissions)
                {
                    message += "* " + perm + Environment.NewLine;
                }

                return message;
            }
        }

        public NotEnoughPermissionsException(ICommandCaller caller, string permission) : this(caller,
            new[] {permission})
        {

        }
    }
}