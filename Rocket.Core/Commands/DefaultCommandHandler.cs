using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;
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

            List<ICommand> tree = new List<ICommand> { context.Command };
            context = GetChild(context, context, tree);

            var permission = GetPermission(context);

#if !DEBUG
            try
            {
#endif
            IPermissionProvider provider = container.Resolve<IPermissionProvider>();

            if (provider.CheckPermission(user, permission) != PermissionResult.Grant)
            {
                var logger = container.Resolve<ILogger>();
                logger.LogInformation($"{user.Name} does not have permissions to execute: \"{commandLine}\"");
                throw new NotEnoughPermissionsException(user, permission);
            }

            context.Command.Execute(context);
#if !DEBUG
            }
            catch (Exception e)
            {
                if (e is ICommandFriendlyException exception)
                {
                    exception.SendErrorMessage(context);
                    return true;
                }

                context.User.SendMessage("An internal error occured.", Color.DarkRed);
                throw new Exception($"Command {commandLine} of user {user.Name} caused an exception: ", e);
            }
#endif

            return true;
        }

        public bool SupportsUser(Type user) => true;

        //Builds a defalt permission
        //If the command is "A" with Child Command "B", the default permission will be "A.B"
        public string GetPermission(ICommandContext context)
        {
            var node = context.RootContext;

            string permission = "";

            while (node != null)
            {
                if (permission == "")
                    permission = node.Command.Name;
                else
                    permission += "." + node.Command.Name;

                node = node.ChildContext;
            }

            return permission;
        }

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