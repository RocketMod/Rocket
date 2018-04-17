using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Permissions;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandPermission : ICommand
    {
        public List<ICommand> ChildCommands => null;

        public string GetSyntax(ICommandContext context)
        {
            if (context.Parameters.Length > 1)
            {
                string cmd = context.Parameters.Get<string>(0).ToLower();
                switch (cmd)
                {
                    case "add":
                    case "remove":
                        return "<player/group> [target] <permission>";

                    case "reload":
                        return "";
                }
            }

            return "<add/remove/reload>";
        }

        public string Description => "Add or remove permissions from players and groups";

        public List<string> Aliases => new List<string> { "P" };

        public string GetHelpText(ICommandContext context)
        {
            if (context.Parameters.Length < 2)
                return Description;

            string cmd = context.Parameters.Get<string>(0).ToLower();
            switch (cmd)
            {
                case "add":
                case "remove":
                    return "Add or remove a permission from a player or group";

                case "reload":
                    return "Reload the permissions from config";
            }

            return "Unknown sub command";
        }

        public string Name => "PermissionGroup";
        public string Permission => "Rocket.Permissions.ManagePermissions";

        public void Execute(ICommandContext context)
        {
            var permissions = context.Container.Get<IPermissionProvider>("default_permissions");
            if (context.Parameters.Length != 3)
                throw new CommandParameterMismatchException();

            var cmd = context.Parameters.Get<string>(0).ToLower();
            var type = context.Parameters.Get<string>(1).ToLower();
            var targetName = context.Parameters.Get<string>(2);
            var permissionToUpdate = context.Parameters.Get<string>(3);

            IPermissible target;
            string permission;
            string permissionFailMessage;
            switch (type)
            {
                case "group":
                    permission = "Rocket.Permissions.ManageGroups." + targetName;
                    permissionFailMessage = "You don't have permissions to manage this group.";

                    target = permissions.GetGroup(targetName);
                    if (target == null)
                    {
                        context.Caller.SendMessage($"Group \"{targetName}\" was not found.", ConsoleColor.Red);
                        return;
                    }
                    break;
                case "player":
                    permission = "Rocket.Permissions.ManagePlayers";
                    permissionFailMessage = "You don't have permissions to manage permissions of players.";

                    target = context.Parameters.Get<IPlayer>(2);
                    break;

                default:
                    throw new CommandWrongUsageException();
            }
            
            if (permissions.CheckPermission(context.Caller, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.Caller, permission, permissionFailMessage);

            switch (cmd)
            {
                case "add":
                    {
                        if (permissions.AddPermission(target, permissionToUpdate))
                        {
                            context.Caller.SendMessage($"Successfully added \"{permissionToUpdate}\" to \"{targetName}\"!", ConsoleColor.DarkGreen);
                        }
                        else
                        {
                            context.Caller.SendMessage($"Failed to add \"{permissionToUpdate}\" to \"{targetName}\"!", ConsoleColor.Red);
                        }
                        break;
                    }
                case "remove":
                    {
                        if (permissions.RemovePermission(target, permissionToUpdate))
                        {
                            context.Caller.SendMessage($"Successfully removed \"{permissionToUpdate}\" from \"{targetName}\"!", ConsoleColor.DarkGreen);
                        }
                        else
                        {
                            context.Caller.SendMessage($"Failed to remove \"{permissionToUpdate}\" from \"{targetName}\"!", ConsoleColor.Red);
                        }
                        break;
                    }
                case "reload":
                    {
                        context.Caller.SendMessage("Permissions have been reloaded.", ConsoleColor.DarkGreen);
                        permissions.Reload();
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