using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Plugin;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandRocket : ICommand
    {
        public string Name => "Rocket";
        public string Permission => "Rocket.ManageRocket";
        public string Syntax => "<reload>";

        public string Description => "Manage Rocket";
        public List<string> Aliases => null;
        public List<ISubCommand> ChildCommands => new List<ISubCommand> { new RocketSubCommandReload() };

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }
    }

    public class RocketSubCommandReload : ISubCommand<CommandPermission>
    {
        public string Name => "Reload";
        public string Description => "Reload Rocket";
        public string Permission => "Rocket.ManageRocket.Reload";
        public string Syntax => "";
        public List<ISubCommand> ChildCommands => null;
        public List<string> Aliases => null;
        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }

        public void Execute(ICommandContext context)
        {
            var permissions = context.Container.Get<IPermissionProvider>();
            permissions.Reload();

            foreach (var plugin in context.Container.Get<IPluginManager>())
            {
                plugin.Reload();
            }

            context.Caller.SendMessage("Reload completed.", ConsoleColor.DarkGreen);
        }
    }
}