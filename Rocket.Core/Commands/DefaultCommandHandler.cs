using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                args[0], args.Skip(1).ToArray(), null, null);

            ICommand target = context.Container.Resolve<ICommandProvider>().Commands.GetCommand(context.CommandAlias, caller);
            if (target == null)
                return false; // only return false when the command was not found

            context.Command = target;

            List<ICommand> tree = new List<ICommand> {target};
            context = GetChild(context, context, tree);

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

            if(Debugger.IsAttached) // go to exception directly in VS
                context.Command.Execute(context);
            else
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

        private CommandContext GetChild(CommandContext root, CommandContext context, List<ICommand> tree)
        {
            if (context.Command?.ChildCommands == null || context.Parameters.Length == 0)
                return context;

            var alias = context.Parameters[0];
            var cmd = context.Command.ChildCommands.GetCommand(alias, context.Caller);

            if (cmd == null)
                return context;

            if (!cmd.SupportsCaller(context.Caller.GetType()))
                throw new NotSupportedException(context.Caller.GetType().Name + " can not use this command.");

            tree.Add(cmd);

            CommandContext childContext = new CommandContext(
                context.Container.CreateChildContainer(),
                context.Caller,
                context.CommandPrefix + context.CommandAlias + " ",
                cmd,
                alias,
                context.Parameters.ToArray().Skip(1).ToArray(),
                context,
                root
            );

            return GetChild(root, childContext, tree);
        }

        private void GuardCaller(ICommandCaller caller)
        {
            if (!SupportsCaller(caller.GetType()))
                throw new NotSupportedException(caller.GetType().FullName + " is not supported!");
        }
    }
}