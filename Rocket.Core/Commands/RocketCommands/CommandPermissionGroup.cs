using System;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Permissions;
using Rocket.Core.User;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandPermissionGroup : ICommand
    {
        public string Syntax => "";

        public IChildCommand[] ChildCommands => new IChildCommand[]
            {new PermissionGroupChildrenCommandAdd(), new PermissionGroupChildrenCommandRemove()};

        public string Summary => "Manages permission groups.";
        public string Description => null;

        public string[] Aliases => new[] {"PG"};

        public string Name => "PermissionGroup";
        public string Permission => "Rocket.Permissions.ManageGroups";

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsUser(Type user)=> true;
    }

    public abstract class PermissionGroupChildrenCommandUpdate : IChildCommand
    {
        public abstract string Name { get; }
        public abstract string Summary { get; }
        public string Description => null;
        public abstract string Permission { get; }
        public string Syntax => "<player> <group>";

        public IChildCommand[] ChildCommands => null;
        public abstract string[] Aliases { get; }

        public bool SupportsUser(Type user)=> true;

        public void Execute(ICommandContext context)
        {
            IPlayer targetPlayer = context.Parameters.Get<IPlayer>(0);
            string groupName = context.Parameters.Get<string>(1);

            string permission = "Rocket.Permissions.ManageGroups." + groupName;
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>("default_permissions");

            if (permissions.CheckPermission(context.User, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.User, permission,
                    "You don't have permissions to manage this group.");

            IPermissionGroup groupToUpdate = permissions.GetGroup(groupName);
            if (groupToUpdate == null)
            {
                context.User.SendMessage($"Group \"{groupName}\" was not found.", Color.Red);
                return;
            }

            UpdateGroup(context.User, permissions, targetPlayer, groupToUpdate);
        }

        protected abstract void UpdateGroup(IUser user, IPermissionProvider permissions,
                                            IPlayer targetPlayer, IPermissionGroup groupToUpdate);
    }

    public class PermissionGroupChildrenCommandAdd : PermissionGroupChildrenCommandUpdate
    {
        public override string Name => "Add";
        public override string Summary => "Adds a player to a permission group.";
        public override string Permission => "Rocket.Permissions.ManageGroups.Add";
        public override string[] Aliases => new[] {"a", "+"};

        protected override void UpdateGroup(IUser user, IPermissionProvider permissions,
                                            IPlayer targetPlayer, IPermissionGroup groupToUpdate)
        {
            if (permissions.AddGroup(targetPlayer, groupToUpdate))
                user.SendMessage($"Successfully added {targetPlayer:Name} to \"{groupToUpdate:Name}\"!",
                    Color.DarkGreen);
            else
                user.SendMessage($"Failed to add {targetPlayer:Name} to \"{groupToUpdate:Name}\"!", Color.Red);
        }
    }

    public class PermissionGroupChildrenCommandRemove : PermissionGroupChildrenCommandUpdate
    {
        public override string Name => "Remove";
        public override string Summary => "Removes a player from a permission group.";
        public override string Permission => "Rocket.Permissions.ManageGroups.Remove";
        public override string[] Aliases => new[] {"r", "-"};

        protected override void UpdateGroup(IUser user, IPermissionProvider permissions,
                                            IPlayer targetPlayer, IPermissionGroup groupToUpdate)
        {
            if (permissions.RemoveGroup(targetPlayer, groupToUpdate))
                user.SendMessage($"Successfully removed {targetPlayer:Name} from \"{groupToUpdate:Name}\"!",
                    Color.DarkGreen);
            else
                user.SendMessage($"Failed to remove {targetPlayer:Name} from \"{groupToUpdate:Name}\"!",
                    Color.Red);
        }
    }
}