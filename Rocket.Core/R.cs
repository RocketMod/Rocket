using Rocket.API.Collections;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;
using Rocket.API.Commands;
using Rocket.API.Event;
using Rocket.API.Event.Command;
using Rocket.API.Exceptions;
using Rocket.API.Extensions;
using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Commands;
using Rocket.API.Providers.Configuration;
using Rocket.API.Utils;
using Rocket.API.Providers.Implementation;
using Rocket.API.Providers.Logging;
using Rocket.API.Providers.Permissions;
using Rocket.API.Providers.Plugins;
using Rocket.API.Providers.Remoting;
using Rocket.API.Providers.Translations;
using Rocket.Core.Commands;
using Rocket.Core.Player;
using Rocket.Core.Providers.Logging;
using Rocket.Core.Providers.Permissions;
using Rocket.Core.Providers.Translation;
using Rocket.Core.Providers;

namespace Rocket.Core
{
    public static class R
    {
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        /*
        private static TranslationList defaultTranslation => new TranslationList
        {
            { "rocket_join_public","{0} connected to the server" },
            { "rocket_leave_public","{0} disconnected from the server"},
            { "command_no_permission","You do not have permissions to execute this command."},
            { "command_rocket_plugins_loaded","Loaded: {0}"},
            { "command_rocket_plugins_unloaded","Unloaded: {0}"},
            { "command_rocket_plugins_failure","Failure: {0}"},
            { "command_rocket_plugins_cancelled","Cancelled: {0}"},
            { "command_rocket_reload_plugin","Reloading {0}"},
            { "command_rocket_not_loaded","The plugin {0} is not loaded"},
            { "command_rocket_unload_plugin","Unloading {0}"},
            { "command_rocket_load_plugin","Loading {0}"},
            { "command_rocket_already_loaded","The plugin {0} is already loaded"},
            { "command_rocket_reload","Reloading Rocket"},
            { "command_p_group_not_found","Group not found"},
            { "command_p_group_player_added","{0} was added to the group {1}"},
            { "command_p_group_player_removed","{0} was removed from from the group {1}"},
            { "command_p_unknown_error","Unknown error"},
            { "command_p_player_not_found","{0} was not found"},
            { "command_p_group_not_found","{1} was not found"},
            { "command_p_duplicate_entry","{0} is already in the group {1}"},
            { "command_p_permissions_reload","Permissions reloaded"},
            { "invalid_character_name","invalid character name"},
            { "command_not_found","Command not found."},
            { "command_cooldown","You have to wait {0} seconds before you can use this command again."}
        };
        */
        public static ProviderManager Providers { get; } = new ProviderManager();
        
        //public static RocketPluginProviderProxy Plugins => Providers.GetProviderProxy<RocketPluginProviderProxy>();
        //public static RocketLoggingProviderProxy Logger => Providers.GetProviderProxy<RocketLoggingProviderProxy>();
        //public static RocketCommandProviderProxy Commands => Providers.GetProviderProxy<RocketCommandProviderProxy>();
        //public static RocketRemotingProviderProxy Remoting => Providers.GetProviderProxy<RocketRemotingProviderProxy>();
        /*
        //todo refactor this but dont use providers? (we have events)
        public static bool Execute(IRocketPlayer player, string command)
        {
            command = command.TrimStart('/');
            string[] commandParts = Regex.Matches(command, @"[\""'](?<1>.*?)[\""']|(?<1>[^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Cast<Match>().Select(m => m.Groups [0].Value.Trim()).ToArray();

            if (commandParts.Length == 0)
                return false;

            string name = commandParts[0];
            string[] parameters = commandParts.Skip(1).ToArray();
            if (player == null) player = new ConsolePlayer();
            IRocketCommand rocketCommand = R.Commands.Commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (rocketCommand == null)
                return false;

            if (rocketCommand.AllowedCaller == AllowedCaller.Player && player is ConsolePlayer)
                return false;
            if (rocketCommand.AllowedCaller == AllowedCaller.Console && !(player is ConsolePlayer))
                return false;

            try
            {
                ExecuteCommandEvent @event = new ExecuteCommandEvent(player, rocketCommand, parameters);
                EventManager.Instance.CallEvent(@event);

                if (@event.IsCancelled)
                {
                    return false;
                }

                try
                {
                    rocketCommand.Execute(new RocketCommandContext(player, parameters, rocketCommand));
                }
                catch (NoPermissionsForCommandException ex)
                {
                    R.Logger.Log(LogLevel.INFO, ex.Message, null, null);
                }
                catch (WrongUsageOfCommandException ex)
                {
                    R.Logger.Log(LogLevel.INFO, ex.Message, null, null);
                }
            }
            catch (Exception ex)
            {
                R.Logger.Log(LogLevel.ERROR, "An error occured while executing " + rocketCommand.Name + " [" + String.Join(", ", parameters) + "]", ex);
            }
            return true;
        }
        */
        internal static GameObject gameObject = new GameObject("Rocket");

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Bootstrap<T>() where T : IGameProvider
        {
            //todo: set working dir to /<server>/Rocket
            //currently it generates everything in unturneds root directory

            //TODO : Load all types then do :
            //Providers.LoadRocketProviders();
            



            /*
             * 
             * FOlgendes gehört NICHT hier her
            Logger.LogMessage(LogLevel.INFO, "####################################################################################", ConsoleColor.Yellow);
            Logger.LogMessage(LogLevel.INFO, "Starting RocketMod " + Version, ConsoleColor.Yellow);
            Logger.LogMessage(LogLevel.INFO, "####################################################################################", ConsoleColor.Yellow);
            
            try
            {
                Providers.registerProvider<T>();
                Providers.LoadFromAssembly(typeof(T).Assembly); // Load implementation providers

                //Providers.registerProvider<RocketBuiltinCommandProvider>();
                //Providers.registerProvider<RocketBuiltinTranslationProvider>();
                //Providers.registerProvider<RocketBuiltinPermissionsProvider>();

                Providers.Load();

                Plugins.LoadPlugins();

                Translations.RegisterDefaultTranslations(defaultTranslation);
                Translations.RegisterDefaultTranslations(Implementation.DefaultTranslation);

                gameObject.TryAddComponent<TaskDispatcher>();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.FATAL, null, ex);
            }*/
        }
    }
}
