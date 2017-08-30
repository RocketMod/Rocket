using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Providers;
using Rocket.API.Plugins;
using Rocket.API.Permissions;

namespace Rocket.Core.Commands
{
    public class CommandRocket : ICommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "rocket";
        public string Help => "Reloading Rocket or individual plugins";
        public string Syntax => "<plugins | reload> | <reload | unload | load> <plugin>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "rocket.info", "rocket.rocket" };

        protected ITranslationProvider translations;
        protected IGameProvider game;
        protected IPluginProvider plugins;
        protected IChatProvider chat;
        protected IPermissionsProvider permissions;
        protected IProviderManager providerManager;

        public CommandRocket(IProviderManager providerManager,ITranslationProvider translationProvider,IPermissionsProvider permissionsProvider, IGameProvider gameProvider, IPluginProvider pluginProvider, IChatProvider chatProvider)
        {
            translations = translationProvider;
            game = gameProvider;
            plugins = pluginProvider;
            chat = chatProvider;
            permissions = permissionsProvider;
        }

        public void Execute(ICommandContext ctx)
        {
            var command = ctx.Parameters;
            var caller = ctx.Caller;

            if (command.Length == 0)
            {
                chat.Say("Rocket v" + Assembly.GetExecutingAssembly().GetName().Version + " for "+ game.Name);
                chat.Say("https://rocketmod.net - 2017");
                return;
            }

            var allPlugins = plugins.Plugins;
            if (command.Length == 1)
            {
                switch (command[0].ToLower()) {
                    case "plugins":
                        if (caller != null && permissions.CheckPermission(caller,"rocket.plugins").Result != PermissionResultType.GRANT ) return;
                        ctx.Output.Print(translations.Translate("command_rocket_plugins_loaded", String.Join(", ", allPlugins.Where(p => p.State == PluginState.Loaded).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        ctx.Output.Print(translations.Translate("command_rocket_plugins_unloaded", String.Join(", ", allPlugins.Where(p => p.State == PluginState.Unloaded).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        ctx.Output.Print(translations.Translate("command_rocket_plugins_failure",  String.Join(", ", allPlugins.Where(p => p.State == PluginState.Failure).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        ctx.Output.Print(translations.Translate("command_rocket_plugins_cancelled", String.Join(", ", allPlugins.Where(p => p.State == PluginState.Cancelled).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        break;
                    case "reload":
                        if (caller!=null && permissions.CheckPermission(caller, "rocket.reload").Result != PermissionResultType.GRANT) return;
                            ctx.Output.Print(translations.Translate("command_rocket_reload"));
                            providerManager.Reload();
                        break;
                }
            }

            if (command.Length == 2)
            {
                IPlugin p = allPlugins.FirstOrDefault(pl => pl.Name.ToLower().Contains(command[1].ToLower()));
                if (p == null)
                {
                    ctx.Output.Print(translations.Translate("command_rocket_plugin_not_found", command[1]));
                    return;
                }

                switch (command[0].ToLower())
                {
                    case "reload":
                        if (caller != null && permissions.CheckPermission(caller, "rocket.reloadplugin").Result != PermissionResultType.GRANT) return;
                        if (p.State == PluginState.Loaded)
                        {
                            chat.Say(caller,
                                translations.Translate("command_rocket_reload_plugin",
                                    p.GetType().Assembly.GetName().Name));
                            p.ReloadPlugin();
                        }
                        else
                        {
                            ctx.Output.Print(translations.Translate("command_rocket_not_loaded",
                                p.GetType().Assembly.GetName().Name));
                        }
                        break;
                    case "unload":
                        if (caller != null && permissions.CheckPermission(caller, "rocket.unloadplugin").Result != PermissionResultType.GRANT) return;
                        if (p.State == PluginState.Loaded)
                        {
                            ctx.Output.Print(translations.Translate("command_rocket_unload_plugin",
                                p.GetType().Assembly.GetName().Name));
                            p.UnloadPlugin();
                        }
                        else
                        {
                            ctx.Output.Print(translations.Translate("command_rocket_not_loaded",
                                p.GetType().Assembly.GetName().Name));
                        }
                        break;
                    case "load":
                        if (caller != null && permissions.CheckPermission(caller, "rocket.loadplugin").Result != PermissionResultType.GRANT) return;
                        if (p.State != PluginState.Loaded)
                        {
                            ctx.Output.Print(translations.Translate("command_rocket_load_plugin",
                                p.GetType().Assembly.GetName().Name));
                            p.LoadPlugin();
                        }
                        else
                        {
                            ctx.Output.Print(translations.Translate("command_rocket_already_loaded",
                                p.GetType().Assembly.GetName().Name));
                        }
                        break;
                }
            }
        }
    }
}