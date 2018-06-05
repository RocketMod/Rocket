using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;
using Rocket.Core.Permissions;
using Rocket.Core.User;

namespace Rocket.Core.Commands
{
    public class DefaultCommandHandler : ICommandHandler
    {
        private readonly IDependencyContainer container;

        public DefaultCommandHandler(IDependencyContainer container)
        {
            this.container = container;
        }

        public bool HandleCommand(IUser user, string commandLine, string prefix)
        {
            GuardUser(user);

            commandLine = commandLine.Trim();
            string[] args = commandLine.Split(' ');

            IDependencyContainer contextContainer = container.CreateChildContainer();
            IRocketSettingsProvider settings = contextContainer.Resolve<IRocketSettingsProvider>();

            if (settings.Settings.Logging.EnableCommandExecutionsLogs)
                contextContainer.Resolve<ILogger>().LogInformation($"{user.Name} executed command: \"{commandLine}\"");

            CommandContext context = new CommandContext(contextContainer,
                user, prefix, null,
                args[0], args.Skip(1).ToArray(), null, null);

            ICommand target = context.Container.Resolve<ICommandProvider>()
                                     .Commands.GetCommand(context.CommandAlias, user);
            if (target == null)
                return false; // only return false when the command was not found

            context.Command = target;

            List<ICommand> tree = new List<ICommand> { target };
            context = GetChild(context, context, tree);

            //Builds a defalt permission
            //If the command is "A" with Child Command "B", the default permission will be "A.B"
            string defaultPerm = "";
            foreach (ICommand node in tree)
                if (defaultPerm == "")
                    defaultPerm = node.Name;
                else
                    defaultPerm += "." + node.Name;

            List<string> tmp = new List<string> { defaultPerm };
            if (context.Command.Permission != null)
                tmp.Add(context.Command.Permission);

            string[] perms = tmp.ToArray();

            IPermissionProvider provider = container.Resolve<IPermissionProvider>();
            if (provider.CheckHasAnyPermission(user, perms) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(user, perms);

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

#if DEBUG
                context.User.SendMessage(e.ToString(), Color.DarkRed);
#else
                throw;
#endif
            }

            return true;
        }

        public bool SupportsUser(Type user) => true;

        private CommandContext GetChild(CommandContext root, CommandContext context, List<ICommand> tree)
        {
            if (context.Command?.ChildCommands == null || context.Parameters.Length == 0)
                return context;

            string alias = context.Parameters[0];
            ICommand cmd = context.Command.ChildCommands.GetCommand(alias, context.User);

            if (cmd == null)
                return context;

            if (!cmd.SupportsUser(context.User.GetType()))
                throw new NotSupportedException(context.User.GetType().Name + " can not use this command.");

            tree.Add(cmd);

            CommandContext childContext = new CommandContext(
                context.Container.CreateChildContainer(),
                context.User,
                context.CommandPrefix + context.CommandAlias + " ",
                cmd,
                alias,
                context.Parameters.ToArray().Skip(1).ToArray(),
                context,
                root
            );

            context.ChildContext = childContext;
            return GetChild(root, childContext, tree);
        }

        private void GuardUser(IUser user)
        {
            if (!SupportsUser(user.GetType()))
                throw new NotSupportedException(user.GetType().FullName + " is not supported!");
        }

        public string ServiceName => "RocketCommandHandler";
    }
}