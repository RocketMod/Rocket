using System;
using System.Collections.Generic;
using Rocket.API.Drawing;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.User;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandHelp : ICommand
    {
        public string Name => "Help";
        public string[] Aliases => new[] {"h"};
        public string Summary => "Provides help for all or a specific command.";
        public string Description => null;
        public string Syntax => "[command] [1. Child Command] [2. Child Command] [...]";
        public IChildCommand[] ChildCommands => null;

        public bool SupportsUser(UserType User) => true;

        public void Execute(ICommandContext context)
        {
            ICommandProvider cmdProvider = context.Container.Resolve<ICommandProvider>();
            ICommandHandler cmdHandler = context.Container.Resolve<ICommandHandler>();

            IPermissionProvider permissionProvider = context.Container.Resolve<IPermissionProvider>();

            string rootPrefix = context.RootContext.CommandPrefix;
            IEnumerable<ICommand> childs = cmdProvider.Commands.OrderBy(c => c.Name);

            if (context.Parameters.Length > 0)
            {
                ICommand cmd = null;
                string prefix = rootPrefix;

                int i = 0;
                foreach (string commandNode in context.Parameters)
                {
                    cmd = childs?.GetCommand(commandNode, context.User);

                    if (cmd == null || !HasAccess(cmd, context.User))
                    {
                        context.User.SendMessage("Command was not found: " + prefix + commandNode, Color.Red);
                        return;
                    }

                    childs = cmd.ChildCommands?.OrderBy(c => c.Name).Cast<ICommand>();
                    if (i != context.Parameters.Length - 1)
                        prefix += commandNode + " ";
                    i++;
                }

                context.User.SendMessage(GetCommandUsage(cmd, prefix), Color.Blue);

                if (cmd.Description != null)
                    context.User.SendMessage(cmd.Description, Color.Cyan);

                List<ICommand> childCommands =
                    (cmd.ChildCommands?.Cast<ICommand>().ToList() ?? new List<ICommand>())
                    .Where(c => HasAccess(c, context.User))
                    .OrderBy(c => c.Name)
                    .ToList();

                if (childCommands.Count == 0)
                    return;

                foreach (ICommand subCmd in childCommands)
                    context.User.SendMessage(GetCommandUsage(subCmd, rootPrefix + cmd.Name.ToLower() + " "),
                        Color.Blue);

                return;
            }

            context.User.SendMessage("Available commands: ", Color.Green);

            foreach (ICommand cmd in cmdProvider.Commands.OrderBy(c => c.Name))
                if (HasAccess(cmd, context.User))
                    context.User.SendMessage(GetCommandUsage(cmd, rootPrefix), Color.Blue);
        }

        public bool HasAccess(ICommand command, IUser user) 
            => command.SupportsUser(user.Type);

        public string GetCommandUsage(ICommand command, string prefix) => prefix
            + command.Name.ToLower()
            + (string.IsNullOrEmpty(command.Syntax) ? "" : " " + command.Syntax)
            + (string.IsNullOrEmpty(command.Summary) ? "" : ": " + command.Summary);
    }
}