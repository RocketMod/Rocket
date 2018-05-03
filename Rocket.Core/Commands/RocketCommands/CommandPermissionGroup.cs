using System;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Permissions;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandPermissionGroup : ICommand
    {
        public string Syntax => "<add/remove> <player> <group>";

        public ISubCommand[] ChildCommands => new ISubCommand[]
            {new PermissionGroupSubCommandAdd(), new PermissionGroupSubCommandRemove()};

        public string Description => "Manages permission groups";

        public string[] Aliases => new[] {"PG"};

        public string Name => "PermissionGroup";
        public string Permission => "Rocket.Permissions.ManageGroups";

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsCaller(Type commandCaller) => true;
    }

    public abstract class PermissionGroupSubCommandUpdate : ISubCommand
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Permission { get; }
        public string Syntax => "<player> <group>";

        public ISubCommand[] ChildCommands => null;
        public abstract string[] Aliases { get; }

        public bool SupportsCaller(Type commandCaller) => true;

        public void Execute(ICommandContext context)
        {
            IPlayer targetPlayer = context.Parameters.Get<IPlayer>(0);
            string groupName = context.Parameters.Get<string>(1);

            string permission = "Rocket.Permissions.ManageGroups." + groupName;
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>("default_permissions");

            if (permissions.CheckPermission(context.Caller, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.Caller, permission,
                    "You don't have permissions to manage this group.");

            IPermissionGroup groupToUpdate = permissions.GetGroup(groupName);
            if (groupToUpdate == null)
            {
                context.Caller.SendMessage($"Group \"{groupName}\" was not found.", ConsoleColor.Red);
                return;
            }

            UpdateGroup(context.Caller, permissions, targetPlayer, groupToUpdate);
        }

        protected abstract void UpdateGroup(ICommandCaller caller, IPermissionProvider permissions,
                                            IPlayer targetPlayer, IPermissionGroup groupToUpdate);
    }

    public class PermissionGroupSubCommandAdd : PermissionGroupSubCommandUpdate
    {
        public override string Name => "Add";
        public override string Description => "Adds a player to a permission group";
        public override string Permission => "Rocket.Permissions.ManageGroups.Add";
        public override string[] Aliases => new[] {"a", "+"};

        protected override void UpdateGroup(ICommandCaller caller, IPermissionProvider permissions,
                                            IPlayer targetPlayer, IPermissionGroup groupToUpdate)
        {
            if (permissions.AddGroup(targetPlayer, groupToUpdate))
                caller.SendMessage($"Successfully added {targetPlayer:Name} to \"{groupToUpdate:Name}\"!",
                    ConsoleColor.DarkGreen);
            else
                caller.SendMessage($"Failed to add {targetPlayer:Name} to \"{groupToUpdate:Name}\"!", ConsoleColor.Red);
        }
    }

    public class PermissionGroupSubCommandRemove : PermissionGroupSubCommandUpdate
    {
        public override string Name => "Remove";
        public override string Description => "Removes a player from a permission group";
        public override string Permission => "Rocket.Permissions.ManageGroups.Remove";
        public override string[] Aliases => new[] {"r", "-"};

        protected override void UpdateGroup(ICommandCaller caller, IPermissionProvider permissions,
                                            IPlayer targetPlayer, IPermissionGroup groupToUpdate)
        {
            if (permissions.RemoveGroup(targetPlayer, groupToUpdate))
                caller.SendMessage($"Successfully removed {targetPlayer:Name} from \"{groupToUpdate:Name}\"!",
                    ConsoleColor.DarkGreen);
            else
                caller.SendMessage($"Failed to remove {targetPlayer:Name} from \"{groupToUpdate:Name}\"!",
                    ConsoleColor.Red);
        }
    }
}