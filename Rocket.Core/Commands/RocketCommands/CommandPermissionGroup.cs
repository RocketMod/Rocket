using System.Threading.Tasks;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.Permissions;
using Rocket.Core.User;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandPermissionGroup : ICommand
    {
        public string Syntax => "";

        public IChildCommand[] ChildCommands => new IChildCommand[]
            {new CommandGroupAdd(), new CommandGroupRemove()};

        public string Summary => "Manages permission groups.";
        public string Description => null;

        public string[] Aliases => new[] { "PG" };

        public string Name => "PermissionGroup";

        public async Task ExecuteAsync(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool  SupportsUser(IUser user) => true;
    }

    public abstract class CommandGroupUpdate : IChildCommand
    {
        public abstract string Name { get; }
        public abstract string Summary { get; }
        public string Description => null;
        public abstract string Permission { get; }
        public string Syntax => "<player> <group>";

        public IChildCommand[] ChildCommands => null;
        public abstract string[] Aliases { get; }

        public bool  SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            IUser targetPlayer = await context.Parameters.GetAsync<IUser>(0);
            string groupName = context.Parameters[1];

            string permission = "Rocket.Permissions.ManageGroups." + groupName;
            IPermissionProvider configPermissions = context.Container.Resolve<IPermissionProvider>("default_permissions");
            IPermissionChecker permissionChecker = context.Container.Resolve<IPermissionChecker>();

            if (await permissionChecker.CheckPermissionAsync(context.User, permission) != PermissionResult.Grant)
                throw new NotEnoughPermissionsException(context.User, permission,
                    "You don't have permissions to manage this group.");

            IPermissionGroup groupToUpdate = await configPermissions.GetGroupAsync(groupName);
            if (groupToUpdate == null)
            {
                await context.User.SendMessageAsync($"Group \"{groupName}\" was not found.", Color.Red);
                return;
            }

            await UpdateGroupAsync(context.User, configPermissions, targetPlayer, groupToUpdate);
        }

        protected abstract Task UpdateGroupAsync(IUser user, IPermissionProvider permissions,
                                            IUser targetUser, IPermissionGroup groupToUpdate);
    }

    public class CommandGroupAdd : CommandGroupUpdate
    {
        public override string Name => "Add";
        public override string Summary => "Adds a player to a permission group.";
        public override string Permission => "Rocket.Permissions.ManageGroups.Add";
        public override string[] Aliases => new[] { "a", "+" };

        protected override async Task UpdateGroupAsync(IUser user, IPermissionProvider permissions,
                                            IUser targetUser, IPermissionGroup groupToUpdate)
        {
            if (await permissions.AddGroupAsync(targetUser, groupToUpdate))
                await user.SendMessageAsync($"Successfully added {targetUser.DisplayName} to \"{groupToUpdate.Name}\"!",
                    Color.DarkGreen);
            else
                await user.SendMessageAsync($"Failed to add {targetUser.DisplayName} to \"{groupToUpdate.Name}\"!", Color.Red);
        }
    }

    public class CommandGroupRemove : CommandGroupUpdate
    {
        public override string Name => "Remove";
        public override string Summary => "Removes a player from a permission group.";
        public override string Permission => "Rocket.Permissions.ManageGroups.Remove";
        public override string[] Aliases => new[] { "r", "-" };

        protected override async Task UpdateGroupAsync(IUser user, IPermissionProvider permissions,
                                            IUser targetUser, IPermissionGroup groupToUpdate)
        {
            if (await permissions.RemoveGroupAsync(targetUser, groupToUpdate))
                await user.SendMessageAsync($"Successfully removed {targetUser.DisplayName} from \"{groupToUpdate.Name}\"!",
                    Color.DarkGreen);
            else
                await user.SendMessageAsync($"Failed to remove {targetUser.DisplayName} from \"{groupToUpdate.Name}\"!",
                    Color.Red);
        }
    }
}