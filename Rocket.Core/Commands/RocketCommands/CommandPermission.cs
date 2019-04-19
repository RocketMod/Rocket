using System.Threading.Tasks;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.Permissions;
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

        public async Task ExecuteAsync(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsUser(IUser user) => true;
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

        public bool SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            if (context.Parameters.Length != 3)
                throw new CommandWrongUsageException();

            IPermissionActor target;
            string permission;
            string permissionFailMessage;

            string type = context.Parameters[0].ToLower();
            string targetName = context.Parameters[1];
            string permissionToUpdate = context.Parameters[2];

            IPermissionProvider configPermissions = context.Container.Resolve<IPermissionProvider>("default_permissions");
            IPermissionChecker permissionChecker = context.Container.Resolve<IPermissionChecker>();

            switch (type)
            {
                case "g":
                case "group":
                    permission = "Rocket.Permissions.ManageGroups." + targetName;
                    permissionFailMessage = "You don't have permissions to manage this group.";

                    target = await configPermissions.GetGroupAsync(targetName);
                    if (target == null)
                    {
                        await context.User.SendMessageAsync($"Group \"{targetName}\" was not found.", Color.Red);
                        return;
                    }

                    break;

                case "p":
                case "player":
                    permission = "Rocket.Permissions.ManagePlayers";
                    permissionFailMessage = "You don't have permissions to manage permissions of players.";
                    target = await context.Parameters.GetAsync<IUser>(1);
                    break;

                default:
                    throw new CommandWrongUsageException();
            }

            if (await permissionChecker.CheckPermissionAsync(context.User, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.User, permission, permissionFailMessage);

            await UpdatePermissionAsync(context.User, configPermissions, target, permissionToUpdate);
        }

        protected abstract Task UpdatePermissionAsync(IUser user, IPermissionProvider permissions,
                                                 IPermissionActor target, string permissionToUpdate);
    }

    public class CommandPermissionAdd : CommandPermissionUpdate
    {
        public override string Name => "Add";
        public override string Summary => "Adds a permission to a group or player.";
        public override string Permission => "Rocket.Permissions.ManagePermissions.Add";
        public override string[] Aliases => new[] {"a", "+"};

        protected override async Task UpdatePermissionAsync(IUser user, IPermissionProvider permissions,
                                                 IPermissionActor target, string permissionToUpdate)
        {
            if (await permissions.AddPermissionAsync(target, permissionToUpdate))
                await user.SendMessageAsync($"Successfully added \"{permissionToUpdate}\" to \"{target.GetDisplayName()}\"!",
                    Color.DarkGreen);
            else
                await user.SendMessageAsync($"Failed to add \"{permissionToUpdate}\" to \"{target.GetDisplayName()}\"!", Color.Red);
        }
    }

    public class CommandPermissionRemove : CommandPermissionUpdate
    {
        public override string Name => "Remove";
        public override string Summary => "Removes permission from a group or player.";
        public override string Permission => "Rocket.Permissions.ManagePermissions.Remove";
        public override string[] Aliases => new[] {"r", "-"};

        protected override async Task UpdatePermissionAsync(IUser user, IPermissionProvider permissions,
                                                 IPermissionActor target, string permissionToUpdate)
        {
            if (await permissions.RemovePermissionAsync(target, permissionToUpdate))
                await user.SendMessageAsync($"Successfully removed \"{permissionToUpdate}\" from \"{target.GetDisplayName()}\"!",
                    Color.DarkGreen);
            else
                await user.SendMessageAsync($"Failed to remove \"{permissionToUpdate}\" from \"{target.GetDisplayName()}\"!",
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

        public bool SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();
            await permissions.ReloadAsync();
            await context.User.SendMessageAsync("Permissions have been reloaded.", Color.DarkGreen);
        }
    }
}