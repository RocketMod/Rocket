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
        public string Summary => "Provides help for all or a specific command.";
        public string Description => null;
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
            IEnumerable<ICommand> childs = cmdProvider.Commands.OrderBy(c => c.Name);

            if (context.Parameters.Length > 0)
            {
                ICommand cmd = null;
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

                    childs = cmd.ChildCommands?.OrderBy(c => c.Name).Cast<ICommand>();
                    if (i != context.Parameters.Length - 1)
                        prefix += commandNode + " ";
                    i++;
                }

                context.Caller.SendMessage(GetCommandUsage(cmd, prefix), ConsoleColor.Blue);

                if(cmd.Description != null)
                    context.Caller.SendMessage(cmd.Description, ConsoleColor.Cyan);

                var childCommands =
                    (cmd.ChildCommands?.Cast<ICommand>().ToList() ?? new List<ICommand>())
                    .Where(c => HasAccess(c, context.Caller, permissionProvider))
                    .OrderBy(c => c.Name)
                    .ToList();

                if (childCommands.Count == 0)
                    return;

                foreach (var subCmd in childCommands)
                {
                    context.Caller.SendMessage(GetCommandUsage(subCmd, rootPrefix + cmd.Name.ToLower() + " "), ConsoleColor.Blue);
                }

                return;
            }

            context.Caller.SendMessage("Available commands: ", ConsoleColor.Green);

            foreach (var cmd in cmdProvider.Commands.OrderBy(c => c.Name))
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
            return prefix + command.Name.ToLower() + (string.IsNullOrEmpty(command.Syntax) ? "" : " " + command.Syntax) + (string.IsNullOrEmpty(command.Summary) ? "" : ": " + command.Summary);
        }
    }
}