using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandHelp : ICommand
    {
        public string Name => "Help";
        public string[] Aliases => new[] { "h" };
        public string Description => "Provides help for all or a specific command";
        public string Permission => "Rocket.Help";
        public string Syntax => "[command] [1. sub command] [2. sub command] [...]";
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

                    if (cmd == null || !HasAccess(cmd, context.Caller, permissionProvider))
                    {
                        context.Caller.SendMessage("Command was not found: " + prefix + commandNode, ConsoleColor.Red);
                        return;
                    }

                    childs = cmd.ChildCommands;
                    if (i != context.Parameters.Length - 1)
                        prefix += commandNode + " ";
                    i++;
                }

                context.Caller.SendMessage(GetCommandUsage(cmd, prefix), ConsoleColor.Blue);

                var childCommands =
                    (cmd.ChildCommands?.Cast<ICommand>().ToList() ?? new List<ICommand>())
                    .Where(c => HasAccess(c, context.Caller, permissionProvider))
                    .ToList();

                if (childCommands.Count == 0)
                    return;

                context.Caller.SendMessage("Sub commands:", ConsoleColor.DarkBlue);
                foreach (var subCmd in childCommands)
                {
                    context.Caller.SendMessage(GetCommandUsage(subCmd, rootPrefix + cmd.Name + " "), ConsoleColor.Cyan);
                }

                return;
            }

            foreach (var cmd in cmdProvider.Commands)
            {
                if (HasAccess(cmd, context.Caller, permissionProvider))
                    context.Caller.SendMessage(GetCommandUsage(cmd, rootPrefix), ConsoleColor.Blue);
            }
        }

        public bool HasAccess(ICommand command, ICommandCaller caller, IPermissionProvider permissionProvider)
        {
            return (permissionProvider.CheckPermission(caller, command.Permission ?? command.Name)
                == PermissionResult.Grant
                && command.SupportsCaller(caller.GetType()));
        }

        public string GetCommandUsage(ICommand command, string prefix)
        {
            return prefix + command.Name + (string.IsNullOrEmpty(command.Syntax) ? "" : " " + command.Syntax) + (string.IsNullOrEmpty(command.Description) ? "" : ": " + command.Description);
        }
    }
}