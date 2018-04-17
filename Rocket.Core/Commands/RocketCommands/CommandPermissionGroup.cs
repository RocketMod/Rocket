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
        public string Syntax { get; }
        public List<ISubCommand> ChildCommands => new List<ISubCommand> { new PermissionGroupSubCommandAdd(), new PermissionGroupSubCommandRemove() };

        public string Description { get; }

        public List<string> Aliases => new List<string> { "PG" };

        public string Name => "PermissionGroup";
        public string Permission => "Rocket.Permissions.ManageGroups";

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }
    }

    public abstract class PermissionGroupSubCommandUpdate : ISubCommand<CommandPermissionGroup>
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Permission { get; }
        public string Syntax => "<player> <group>";

        public List<ISubCommand> ChildCommands => null;
        public abstract List<string> Aliases { get; }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }

        public void Execute(ICommandContext context)
        {
            var targetPlayer = context.Parameters.Get<IPlayer>(0);
            var groupName = context.Parameters.Get<string>(1);

            string permission = "Rocket.Permissions.ManageGroups." + groupName;
            var permissions = context.Container.Get<IPermissionProvider>("default_permissions");

            if (permissions.CheckPermission(context.Caller, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.Caller, permission, "You don't have permissions to manage this group.");

            var groupToUpdate = permissions.GetGroup(groupName);
            if (groupToUpdate == null)
            {
                context.Caller.SendMessage($"Group \"{groupName}\" was not found.", ConsoleColor.Red);
                return;
            }

            UpdateGroup(context.Caller, permissions, targetPlayer, groupToUpdate);
        }

        protected abstract void UpdateGroup(ICommandCaller caller, IPermissionProvider permissions, IPlayer targetPlayer, IPermissionGroup groupToUpdate);
    }

    public class PermissionGroupSubCommandAdd : PermissionGroupSubCommandUpdate
    {
        public override string Name => "Add";
        public override string Description => "Add a player to a permission group";
        public override string Permission => "Rocket.Permissions.ManageGroups.Add";
        public override List<string> Aliases => new List<string> { "a", "+" };
        protected override void UpdateGroup(ICommandCaller caller, IPermissionProvider permissions, IPlayer targetPlayer, IPermissionGroup groupToUpdate)
        {
            if (permissions.AddGroup(targetPlayer, groupToUpdate))
                caller.SendMessage($"Successfully added {targetPlayer:Name} to \"{groupToUpdate:Name}\"!", ConsoleColor.DarkGreen);
            else
                caller.SendMessage($"Failed to add {targetPlayer:Name} to \"{groupToUpdate:Name}\"!", ConsoleColor.Red);
        }
    }

    public class PermissionGroupSubCommandRemove : PermissionGroupSubCommandUpdate
    {
        public override string Name => "Remove";
        public override string Description => "Remove a player from a permission group";
        public override string Permission => "Rocket.Permissions.ManageGroups.Remove";
        public override List<string> Aliases => new List<string> { "r", "-" };
        protected override void UpdateGroup(ICommandCaller caller, IPermissionProvider permissions, IPlayer targetPlayer, IPermissionGroup groupToUpdate)
        {
            if (permissions.RemoveGroup(targetPlayer, groupToUpdate))
                caller.SendMessage($"Successfully removed {targetPlayer:Name} from \"{groupToUpdate:Name}\"!", ConsoleColor.DarkGreen);
            else
                caller.SendMessage($"Failed to remove {targetPlayer:Name} from \"{groupToUpdate:Name}\"!", ConsoleColor.Red);
        }
    }
}