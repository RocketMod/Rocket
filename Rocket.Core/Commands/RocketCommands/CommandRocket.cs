using System;
using System.Drawing;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Plugins;
using Rocket.Core.Plugins.NuGet;
using Rocket.Core.User;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandRocket : ICommand
    {
        public string Name => "Rocket";
        public string Permission => "Rocket.ManageRocket";
        public string Syntax => "<reload/install/uninstall>";
        public string Summary => "Manages RocketMod.";
        public string Description => null;
        public string[] Aliases => null;
        public IChildCommand[] ChildCommands => new IChildCommand[] {new CommandRocketInstall(), new CommandRocketInstall.CommandRocketUninstall(),
                                                                     new CommandRocketReload()};

        public void Execute(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsUser(Type user) => true;
    }

    public class CommandRocketReload : IChildCommand
    {
        public string Name => "Reload";
        public string Summary => "Reloads RocketMod and all plugins.";
        public string Description => null;
        public string Permission => "Rocket.ManageRocket.Reload";
        public string Syntax => "";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;

        public bool SupportsUser(Type user) => true;

        public void Execute(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();
            permissions.Reload();

            foreach (IPlugin plugin in context.Container.Resolve<IPluginManager>()) plugin.Unload();

            foreach (IPlugin plugin in context.Container.Resolve<IPluginManager>()) plugin.Load(true);

            context.User.SendMessage("Reload completed.", Color.DarkGreen);
        }
    }

    public class CommandRocketInstall : IChildCommand
    {
        public string Name => "Install";
        public string[] Aliases => null;
        public string Summary => "Installs a plugin";
        public string Description => null;
        public string Permission => "Rocket.ManageRocket.Install";
        public string Syntax => "<repo> <plugin> [version]";
        public IChildCommand[] ChildCommands => null;

        public bool SupportsUser(Type user)
        {
            return true;
        }

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length < 2)
                throw new CommandWrongUsageException();

            NuGetPluginManager pm = context.Container.Resolve<NuGetPluginManager>("nuget_plugins");

            string repoName = context.Parameters.Get<string>(0);
            string pluginName = context.Parameters.Get<string>(1);
            string version;
            context.Parameters.TryGet(1, out version);

            var repo = pm.Repositories.FirstOrDefault(c
                => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));

            if (repo == null)
            {
                context.User.SendMessage("Repository not found: " + repoName, Color.DarkRed);
                return;
            }

            if (pm.Install(repoName, pluginName, version))
            {
                context.User.SendMessage("Successfully installed: " + pluginName, Color.DarkGreen);
            }
            else
            {
                context.User.SendMessage("Failed to install: " + pluginName, Color.DarkRed);
            }
        }

        public class CommandRocketUninstall : IChildCommand
        {
            public string Name => "Uninstall";
            public string[] Aliases => null;
            public string Summary => "Uninstalls plugin";
            public string Description => null;
            public string Permission => "Rocket.ManageRocket.Uninstall";
            public string Syntax => "<repo> <plugin>";
            public IChildCommand[] ChildCommands => null;

            public bool SupportsUser(Type user)
            {
                return true;
            }

            public void Execute(ICommandContext context)
            {
                if (context.Parameters.Length != 2)
                    throw new CommandWrongUsageException();

                NuGetPluginManager pm = context.Container.Resolve<NuGetPluginManager>("nuget_plugins");

                string repoName = context.Parameters.Get<string>(0);
                string pluginName = context.Parameters.Get<string>(1);

                var repo = pm.Repositories.FirstOrDefault(c
                    => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));

                if (repo == null)
                {
                    context.User.SendMessage("Repository not found: " + repoName, Color.DarkRed);
                    return;
                }

                if (pm.Uninstall(repoName, pluginName))
                {
                    context.User.SendMessage("Successfully uninstalled: " + pluginName, Color.DarkGreen);
                }
                else
                {
                    context.User.SendMessage("Failed to uninstall: " + pluginName, Color.DarkRed);
                }
            }
        }
    }
}