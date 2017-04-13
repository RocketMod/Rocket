using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.Providers.Plugins;

namespace Rocket.Core.Commands
{
    public class CommandRocket : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "rocket";
        public string Help => "Reloading Rocket or individual plugins";
        public string Syntax => "<plugins | reload> | <reload | unload | load> <plugin>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "rocket.info", "rocket.rocket" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                R.Implementation.Chat.Say(caller, "Rocket v" + Assembly.GetExecutingAssembly().GetName().Version + " for "+R.Implementation.Name);
                R.Implementation.Chat.Say(caller, "https://rocketmod.net - 2017");
                return;
            }

            string language = "";

            var allPlugins = R.Plugins.GetPlugins();
            if (command.Length == 1)
            {
                switch (command[0].ToLower()) {
                    case "plugins":
                        if (caller != null && !caller.HasPermission("rocket.plugins")) return;
                        R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_plugins_loaded", language, String.Join(", ", allPlugins.Where(p => p.State == PluginState.Loaded).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_plugins_unloaded", language, String.Join(", ", allPlugins.Where(p => p.State == PluginState.Unloaded).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_plugins_failure", language, String.Join(", ", allPlugins.Where(p => p.State == PluginState.Failure).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_plugins_cancelled", language, String.Join(", ", allPlugins.Where(p => p.State == PluginState.Cancelled).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        break;
                    case "reload":
                        if (caller!=null && !caller.HasPermission("rocket.reload")) return;
                            R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_reload", language));
                            R.Reload();
                        break;
                }
            }

            if (command.Length == 2)
            {
                IRocketPlugin p = allPlugins.FirstOrDefault(pl => pl.Name.ToLower().Contains(command[1].ToLower()));
                if (p != null)
                {
                    switch (command[0].ToLower())
                    {
                        case "reload":
                            if (caller != null && !caller.HasPermission("rocket.reloadplugin")) return;
                            if (p.State == PluginState.Loaded)
                            {
                                R.Implementation.Chat.Say(caller,R.Translations.Translate("command_rocket_reload_plugin", language, p.GetType().Assembly.GetName().Name));
                                p.ReloadPlugin();
                            }
                            else
                            {
                                R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_not_loaded", language, p.GetType().Assembly.GetName().Name));
                            }
                            break;
                        case "unload":
                            if (caller != null && !caller.HasPermission("rocket.unloadplugin")) return;
                            if (p.State == PluginState.Loaded)
                            {
                                R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_unload_plugin", language, p.GetType().Assembly.GetName().Name));
                                p.UnloadPlugin();
                            }
                            else
                            {
                                R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_not_loaded", language, p.GetType().Assembly.GetName().Name));
                            }
                            break;
                        case "load":
                            if (caller != null && !caller.HasPermission("rocket.loadplugin")) return;
                            if (p.State != PluginState.Loaded)
                            {
                                R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_load_plugin", language, p.GetType().Assembly.GetName().Name));
                                p.LoadPlugin();
                            }
                            else
                            {
                                R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_already_loaded", language, p.GetType().Assembly.GetName().Name));
                            }
                            break;
                    }
                }
                else
                {
                    R.Implementation.Chat.Say(caller, R.Translations.Translate("command_rocket_plugin_not_found", language, command[1]));
                }
            }
        }
    }
}