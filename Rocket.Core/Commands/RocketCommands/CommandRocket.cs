using System;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Plugins;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandRocket : ICommand
    {
        public string Name => "Rocket";
        public string Permission => "Rocket.ManageRocket";
        public string Syntax => "<reload>";
        public string Summary => "Manages RocketMod.";
        public string Description => null;
        public string[] Aliases => null;
        public IChildCommand[] ChildCommands => new IChildCommand[] {new RocketChildrenCommandReload()};

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsCaller(Type User) => true;
    }

    public class RocketChildrenCommandReload : IChildCommand
    {
        public string Name => "Reload";
        public string Summary => "Reloads RocketMod and all plugins.";
        public string Description => null;
        public string Permission => "Rocket.ManageRocket.Reload";
        public string Syntax => "";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;

        public bool SupportsCaller(Type User) => true;

        public void Execute(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();
            permissions.Reload();

            foreach (IPlugin plugin in context.Container.Resolve<IPluginManager>()) plugin.Reload();

            context.Caller.SendMessage("Reload completed.", ConsoleColor.DarkGreen);
        }
    }
}