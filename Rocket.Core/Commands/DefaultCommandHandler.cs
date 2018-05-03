using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
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

            IDependencyContainer contextContainer = container.CreateChildContainer();

            CommandContext context = new CommandContext(contextContainer,
                caller, prefix, null,
                args[0], args.Skip(1).ToArray(), null);

            ICommand target = GetCommand(context);
            if (target == null)
                return false; // only return false when the command was not found

            context.Command = target;

            List<ICommand> tree = new List<ICommand> {target};
            context = GetChild(context, tree);

            //Builds a defalt permission
            //If the command is "A" with sub command "B", the default permission will be "A.B"
            string defaultPerm = "";
            foreach (ICommand node in tree)
                if (defaultPerm == "")
                    defaultPerm = node.Name;
                else
                    defaultPerm += "." + node.Name;

            List<string> tmp = new List<string> {defaultPerm};
            if (context.Command.Permission != null)
                tmp.Add(context.Command.Permission);

            string[] perms = tmp.ToArray();

            IPermissionProvider provider = container.Resolve<IPermissionProvider>();
            if (provider.CheckHasAnyPermission(caller, perms) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(caller, perms);

            try
            {
                context.Command.Execute(context);
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

        public bool SupportsCaller(Type commandCaller) => true;

        public ICommand GetCommand(ICommandContext context)
        {
            GuardCaller(context.Caller);

            IEnumerable<ICommand> commands = container.Resolve<ICommandProvider>().Commands;
            return commands
                   .Where(c => c.SupportsCaller(context.Caller.GetType()))
                   .FirstOrDefault(c => Equals(c, context.CommandAlias));
        }

        private CommandContext GetChild(CommandContext parent, List<ICommand> tree)
        {
            if (parent.Command?.ChildCommands == null || parent.Parameters.Length == 0)
                return parent;

            foreach (ISubCommand cmd in parent.Command.ChildCommands)
            {
                string alias = parent.Parameters[0];
                if (Equals(cmd, alias))
                {
                    if (!cmd.SupportsCaller(parent.Caller.GetType()))
                        throw new NotSupportedException(parent.Caller.GetType().Name + " can not use this command.");

                    tree.Add(cmd);

                    CommandContext childContext = new CommandContext(
                        parent.Container.CreateChildContainer(),
                        parent.Caller,
                        parent.CommandPrefix + parent.CommandAlias + " ",
                        cmd,
                        alias,
                        ((CommandParameters) parent.Parameters).Parameters.Skip(1).ToArray(),
                        parent
                    );

                    return GetChild(childContext, tree);
                }
            }

            return parent;
        }

        private bool Equals(ICommand command, string alias)
        {
            return command.Name.Equals(alias, StringComparison.OrdinalIgnoreCase)
                || (command.Aliases?.Any(x => x.Equals(alias, StringComparison.OrdinalIgnoreCase)) ?? false);
        }

        private void GuardCaller(ICommandCaller caller)
        {
            if (!SupportsCaller(caller.GetType()))
                throw new NotSupportedException(caller.GetType().FullName + " is not supported!");
        }
    }
}