using System;
using Rocket.API.Drawing;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Permissions;
using Rocket.Core.User;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandPermission : ICommand
    {
        public string Name => "Permission";
        public string Syntax => "";
        public string Summary => "Manages rocket permissions.";
        public string Description => null;

        public IChildCommand[] ChildCommands => new IChildCommand[]
        {
            new CommandPermissionAdd(), new CommandPermissionRemove(),
            new CommandPermissionReload()
        };

        public string[] Aliases => new[] {"P"};

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsUser(Type user) => true;
    }

    public abstract class CommandPermissionUpdate : IChildCommand
    {
        public abstract string Name { get; }
        public abstract string Summary { get; }
        public string Description => null;
        public abstract string Permission { get; }
        public string Syntax => "<[p]layer/[g]roup> [target] [permission]";

        public IChildCommand[] ChildCommands => null;
        public abstract string[] Aliases { get; }

        public bool SupportsUser(Type user) => true;

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length != 3)
                throw new CommandWrongUsageException();

            IIdentity target;
            string permission;
            string permissionFailMessage;

            string type = context.Parameters.Get<string>(0).ToLower();
            string targetName = context.Parameters.Get<string>(1);
            string permissionToUpdate = context.Parameters.Get<string>(2);

            IPermissionProvider configPermissions = context.Container.Resolve<IPermissionProvider>("default_permissions");
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();

            switch (type)
            {
                case "g":
                case "group":
                    permission = "Rocket.Permissions.ManageGroups." + targetName;
                    permissionFailMessage = "You don't have permissions to manage this group.";

                    target = configPermissions.GetGroup(targetName);
                    if (target == null)
                    {
                        context.User.SendMessage($"Group \"{targetName}\" was not found.", Color.Red);
                        return;
                    }

                    break;

                case "p":
                case "player":
                    permission = "Rocket.Permissions.ManagePlayers";
                    permissionFailMessage = "You don't have permissions to manage permissions of players.";
                    target = context.Parameters.Get<IUserInfo>(1);
                    break;

                default:
                    throw new CommandWrongUsageException();
            }

            if (permissions.CheckPermission(context.User, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.User, permission, permissionFailMessage);

            UpdatePermission(context.User, configPermissions, target, permissionToUpdate);
        }

        protected abstract void UpdatePermission(IUser user, IPermissionProvider permissions,
                                                 IIdentity target, string permissionToUpdate);
    }

    public class CommandPermissionAdd : CommandPermissionUpdate
    {
        public override string Name => "Add";
        public override string Summary => "Adds a permission to a group or player.";
        public override string Permission => "Rocket.Permissions.ManagePermissions.Add";
        public override string[] Aliases => new[] {"a", "+"};

        protected override void UpdatePermission(IUser user, IPermissionProvider permissions,
                                                 IIdentity target, string permissionToUpdate)
        {
            if (permissions.AddPermission(target, permissionToUpdate))
                user.SendMessage($"Successfully added \"{permissionToUpdate}\" to \"{target.Name}\"!",
                    Color.DarkGreen);
            else
                user.SendMessage($"Failed to add \"{permissionToUpdate}\" to \"{target.Name}\"!", Color.Red);
        }
    }

    public class CommandPermissionRemove : CommandPermissionUpdate
    {
        public override string Name => "Remove";
        public override string Summary => "Removes permission from a group or player.";
        public override string Permission => "Rocket.Permissions.ManagePermissions.Remove";
        public override string[] Aliases => new[] {"r", "-"};

        protected override void UpdatePermission(IUser user, IPermissionProvider permissions,
                                                 IIdentity target, string permissionToUpdate)
        {
            if (permissions.RemovePermission(target, permissionToUpdate))
                user.SendMessage($"Successfully removed \"{permissionToUpdate}\" from \"{target.Name}\"!",
                    Color.DarkGreen);
            else
                user.SendMessage($"Failed to remove \"{permissionToUpdate}\" from \"{target.Name}\"!",
                    Color.Red);
        }
    }

    public class CommandPermissionReload : IChildCommand
    {
        public string Name => "Reload";
        public string Summary => "Reloads permissions.";
        public string Description => null;
        public string Syntax => "";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => new[] {"R"};

        public bool SupportsUser(Type user) => true;

        public void Execute(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();
            permissions.Reload();
            context.User.SendMessage("Permissions have been reloaded.", Color.DarkGreen);
        }
    }
}