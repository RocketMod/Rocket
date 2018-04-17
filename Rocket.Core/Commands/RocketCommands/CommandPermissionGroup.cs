using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Permissions;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandPermissionGroup : ICommand
    {
        public List<ICommand> ChildCommands { get; }

        public string GetSyntax(ICommandContext context)
        {
            return "[add/remove] [player] [group]";
        }

        public string Description { get; }

        public List<string> Aliases => new List<string> { "PG" };
        public string HelpText => "Add or remove a player from a group.";
        public string GetHelpText(ICommandContext context) => Description;

        public string Name => "PermissionGroup";
        public string Permission => "Rocket.Permissions.ManageGroups";

        public void Execute(ICommandContext context)
        {
            var permissions = context.Container.Get<IPermissionProvider>("default_permissions");
            if (context.Parameters.Length != 3)
                throw new CommandParameterMismatchException();

            var cmd = context.Parameters.Get<string>(0).ToLower();
            var targetPlayer = context.Parameters.Get<IPlayer>(1);
            var groupName = context.Parameters.Get<string>(2);

            string permission = "Rocket.Permissions.ManageGroups." + groupName;
            if (permissions.HasPermission(context.Caller, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.Caller, permission, "You don't have permissions to manage this group.");

            var group = permissions.GetGroup(groupName);
            if (group == null)
            {
                context.Caller.SendMessage($"Group \"{groupName}\" was not found.", ConsoleColor.Red);
                return;
            }

            switch (cmd)
            {
                case "add":
                    {
                        if (permissions.AddGroup(targetPlayer, group))
                            context.Caller.SendMessage($"Successfully added {targetPlayer:Name} to \"{group:Name}\"!", ConsoleColor.DarkGreen);
                        else
                            context.Caller.SendMessage($"Failed to add {targetPlayer:Name} to \"{group:Name}\"!", ConsoleColor.Red);
                        break;
                    }
                case "remove":
                    {
                        if(permissions.RemoveGroup(targetPlayer, group))
                            context.Caller.SendMessage($"Successfully removed {targetPlayer:Name} from \"{group:Name}\"!", ConsoleColor.DarkGreen);
                        else
                            context.Caller.SendMessage($"Failed to remove {targetPlayer:Name} from \"{group:Name}\"!", ConsoleColor.Red);
                        break;
                    }
                default:
                    throw new CommandWrongUsageException("Unknown command syntax: " + context.Parameters[0]);
            }
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }
    }
}