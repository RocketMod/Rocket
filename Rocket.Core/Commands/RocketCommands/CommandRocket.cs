using System;
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
        public string[] Aliases => null;
        public ISubCommand[] ChildCommands => new ISubCommand[] {new RocketSubCommandReload()};

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsCaller(Type commandCaller) => true;
    }

    public class RocketSubCommandReload : ISubCommand
    {
        public string Name => "Reload";
        public string Description => "Reload Rocket";
        public string Permission => "Rocket.ManageRocket.Reload";
        public string Syntax => "";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;

        public bool SupportsCaller(Type commandCaller) => true;

        public void Execute(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Get<IPermissionProvider>();
            permissions.Reload();

            foreach (IPlugin plugin in context.Container.Get<IPluginManager>()) plugin.Reload();

            context.Caller.SendMessage("Reload completed.", ConsoleColor.DarkGreen);
        }
    }
}