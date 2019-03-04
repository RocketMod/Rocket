using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Drawing;
using Rocket.API.Permissions;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.Plugins.NuGet;
using Rocket.Core.User;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Rocket.NuGet;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandRocket : ICommand
    {
        public string Name => "Rocket";
        public string Syntax => "";
        public string Summary => "Manages RocketMod.";
        public string Description => null;
        public string[] Aliases => null;

        public IChildCommand[] ChildCommands => new IChildCommand[]
        {
            new CommandRocketInstall(), new CommandRocketUninstall(),
            new CommandRocketReload(), new CommandRocketUpdate(),
            new CommandRocketVersion()
        };

        public async Task ExecuteAsync(ICommandContext context)
        {
            throw new CommandWrongUsageException();
        }

        public bool SupportsUser(IUser user) => true;
    }

    public class CommandRocketReload : IChildCommand
    {
        public string Name => "Reload";
        public string Summary => "Reloads RocketMod and all plugins.";
        public string Description => null;
        public string Syntax => "";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;

        public bool SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();
            await permissions.ReloadAsync();

            foreach (IPlugin plugin in context.Container.Resolve<IPluginLoader>())
                await plugin.DeactivateAsync();

            foreach (IPlugin plugin in context.Container.Resolve<IPluginLoader>())
                await plugin.ActivateAsync(true);

            await context.User.SendMessageAsync("Reload completed.", Color.DarkGreen);
        }
    }

    public class CommandRocketInstall : IChildCommand
    {
        public string Name => "Install";
        public string[] Aliases => null;
        public string Summary => "Installs a package.";
        public string Description => null;
        public string Syntax => "<plugin> [repo] [version] [-Pre]";
        public IChildCommand[] ChildCommands => null;

        public bool SupportsUser(IUser user) => true;
        public async Task ExecuteAsync(ICommandContext context)
        {
            if (context.Parameters.Length < 1 || context.Parameters.Length > 3)
                throw new CommandWrongUsageException();

            NuGetPluginLoader pm = (NuGetPluginLoader)context.Container.Resolve<IPluginLoader>("nuget_plugins");

            var args = context.Parameters.ToList();

            string packageName = args[0];
            string repoName = null;
            string version = null;
            bool isPre = false;

            if (args.Contains("-Pre"))
            {
                isPre = true;
                args.Remove("-Pre");
            }

            if (args.Count > 1)
                repoName = args[1];

            if (args.Count > 2)
                version = args[2];


            if (repoName != null)
            {
                var repo = pm.Repositories.FirstOrDefault(c => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));

                if (repo == null)
                {
                    await context.User.SendMessageAsync("Repository not found: " + repoName, Color.DarkRed);
                    return;
                }
            }

            var result = await pm.InstallAsync(packageName, version, repoName, isPre);
            if (result.Code != NuGetInstallCode.Success)
            {
                await context.User.SendMessageAsync($"Failed to install \"{packageName}\": " + result, Color.DarkRed);
                return;
            }

            await pm.LoadPluginFromNugetAsync(packageName);
            await context.User.SendMessageAsync($"Successfully installed {packageName} v{result.InstalledVersion}.", Color.DarkGreen);
        }
    }

    public class CommandRocketUninstall : IChildCommand
    {
        public string Name => "Uninstall";
        public string[] Aliases => new []{ "Remove" };
        public string Summary => "Uninstalls a package.";
        public string Description => null;
        public string Syntax => "<package>";
        public IChildCommand[] ChildCommands => null;

        public bool SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            if (context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            NuGetPluginLoader pm = (NuGetPluginLoader)context.Container.Resolve<IPluginLoader>("nuget_plugins");

            string packageName = context.Parameters[0];

            if (!await pm.UninstallAsync(packageName))
            {
                await context.User.SendMessageAsync($"Failed to uninstall \"{packageName}\"", Color.DarkRed);
                return;
            }

            await context.User.SendMessageAsync($"Successfully uninstalled  \"{packageName}\"", Color.DarkGreen);
            await context.User.SendMessageAsync("Restart server to finish uninstall.", Color.Red);
        }
    }

    public class CommandRocketUpdate : IChildCommand
    {
        public string Name => "Update";
        public string[] Aliases => null;
        public string Summary => "Updates a package.";
        public string Description => null;
        public string Syntax => "<plugin> [repo] [version] [-Pre]";
        public IChildCommand[] ChildCommands => null;

        public bool SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            if (context.Parameters.Length < 1 || context.Parameters.Length > 3)
                throw new CommandWrongUsageException();

            NuGetPluginLoader pm = (NuGetPluginLoader)context.Container.Resolve<IPluginLoader>("nuget_plugins");

            var args = context.Parameters.ToList();

            string pluginName = args[0];
            string repoName = null;
            string version = null;
            bool isPre = false;

            if (args.Contains("-Pre"))
            {
                isPre = true;
                args.Remove("-Pre");
            }

            if (args.Count > 1)
                repoName = args[1];

            if (args.Count > 2)
                version = args[2];

            if (repoName != null)
            {
                var repo = pm.Repositories.FirstOrDefault(c
                    => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));

                if (repo == null)
                {
                    await context.User.SendMessageAsync("Repository not found: " + repoName, Color.DarkRed);
                    return;
                }
            }

            var result = await pm.UpdateAsync(pluginName, version, repoName, isPre);
            if (result.Code != NuGetInstallCode.Success)
            {
                await context.User.SendMessageAsync($"Failed to update \"{pluginName}\": " + result, Color.DarkRed);
                return;
            }

            await context.User.SendMessageAsync($"Successfully updated {pluginName} to {result.InstalledVersion}", Color.DarkGreen);
            await context.User.SendMessageAsync("Restart server to finish update.", Color.Red);
        }
    }

    public class CommandRocketVersion : IChildCommand
    {
        public string Name => "Version";
        public string[] Aliases => new[] { "v" };
        public string Summary => "RocketMod version";
        public string Description => null;
        public string Syntax => "";
        public IChildCommand[] ChildCommands => null;
        public bool SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            var runtime = context.Container.Resolve<IRuntime>();
            var host = context.Container.Resolve<IHost>();

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(typeof(IRuntime).Assembly.Location);

            await context.User.SendMessageAsync("Rocket Version: " + versionInfo.FileVersion, Color.Cyan);
            await context.User.SendMessageAsync(runtime.Name + " Version: " + runtime.Version, Color.Cyan);
            await context.User.SendMessageAsync(host.Name + " Version: " + host.HostVersion, Color.Blue);

            if (host.Name != host.GameName)
                await context.User.SendMessageAsync(host.GameName + " Version: " + host.GameVersion, Color.Green);
        }
    }
}