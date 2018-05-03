using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandHelp : ICommand
    {
        public string Name => "Help";
        public string[] Aliases => new[] { "h" };
        public string Description => "Provides help";
        public string Permission => "Rocket.Help";
        public string Syntax => "[command]";
        public ISubCommand[] ChildCommands => null;
        public bool SupportsCaller(Type commandCaller)
        {
            return true;
        }

        public void Execute(ICommandContext context)
        {
            var cmdProvider = context.Container.Resolve<ICommandProvider>();
            var permissionProvider = context.Container.Resolve<IPermissionProvider>();

            var rootPrefix = context.RootCommandContext.CommandPrefix;

            if (context.Parameters.Length > 0)
            {
                ICommand cmd = null;
                IEnumerable<ICommand> childs = cmdProvider.Commands;
                string prefix = rootPrefix;

                int i = 0;
                foreach (string commandNode in context.Parameters)
                {
                    cmd = childs?.GetCommand(commandNode, context.Caller);

                    bool hasPermission = false;
                    if (cmd != null)
                    {
                        hasPermission = (permissionProvider.CheckPermission(context.Caller, cmd.Permission ?? cmd.Name)
                            == PermissionResult.Grant
                            && cmd.SupportsCaller(context.Caller.GetType()));
                    }

                    if (cmd == null || !hasPermission)
                    {
                        throw new CommandWrongUsageException("(Sub-)Command was not found: " + prefix);
                    }

                    childs = cmd.ChildCommands;
                    if (i != context.Parameters.Length - 1)
                        prefix += commandNode + " ";
                    i++;
                }

                context.Caller.SendMessage(GetCommandUsage(cmd, prefix), ConsoleColor.Blue);

                if (cmd.ChildCommands == null || cmd.ChildCommands.Length == 0)
                    return;

                context.Caller.SendMessage("Sub commands:", ConsoleColor.DarkBlue);
                foreach (var subCmd in cmd.ChildCommands)
                {
                    context.Caller.SendMessage(GetCommandUsage(subCmd, rootPrefix + cmd.Name + " "), ConsoleColor.Cyan);
                }

                return;
            }

            foreach (var cmd in cmdProvider.Commands)
            {
                if (permissionProvider.CheckPermission(context.Caller, cmd.Permission ?? cmd.Name) == PermissionResult.Grant
                    && cmd.SupportsCaller(context.Caller.GetType()))
                    context.Caller.SendMessage(GetCommandUsage(cmd, rootPrefix), ConsoleColor.Blue);
            }
        }

        public string GetCommandUsage(ICommand command, string prefix)
        {
            return prefix + command.Name + (string.IsNullOrEmpty(command.Syntax) ? "" : " " + command.Syntax) + (string.IsNullOrEmpty(command.Description) ? "" : ": " + command.Description);
        }
    }
}