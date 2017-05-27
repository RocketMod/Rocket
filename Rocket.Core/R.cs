using Rocket.API.Collections;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Rocket.API.Extensions;
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
using Rocket.Core.Managers;
using Rocket.Core.Providers.Commands;
using Rocket.Core.Providers.Logging;
using Rocket.Core.Providers.Permissions;
using Rocket.Core.Providers.Plugin;
using Rocket.Core.Providers.Translation;

namespace Rocket.Core
{
    public static class R
    {
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

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

        public static RocketProviderManager Providers { get; } = new RocketProviderManager();

        public static IRocketImplementationProvider Implementation => Providers.GetProviders<IRocketImplementationProvider>().First();
        public static IRocketTranslationDataProvider Translations => Providers.GetProviders<IRocketTranslationDataProvider>().First();
        public static IRocketConfigurationDataProvider Configuration => Providers.GetProviders<IRocketConfigurationDataProvider>().First();
        public static IRocketPermissionsDataProvider Permissions => Providers.GetProviders<IRocketPermissionsDataProvider>().First();

        public static RocketPluginProviderProxy Plugins => Providers.GetProviderProxy<RocketPluginProviderProxy>();
        public static RocketLoggingProviderProxy Logger => Providers.GetProviderProxy<RocketLoggingProviderProxy>();
        public static RocketCommandProviderProxy Commands => Providers.GetProviderProxy<RocketCommandProviderProxy>();
        public static RocketRemotingProviderProxy Remoting => Providers.GetProviderProxy<RocketRemotingProviderProxy>();

        public static void Reload()
        {
            try
            {
                Providers.Unload();
                Providers.Load();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        public static void Shutdown()
        {
            try
            {
                Providers.Unload();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        internal static GameObject gameObject = new GameObject("Rocket");

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Bootstrap<T>() where T : IRocketImplementationProvider, IRocketProviderBase
        {
            Logger.Info("####################################################################################");
            Logger.Info("Starting RocketMod " + R.Version);
            Logger.Info("####################################################################################");

            try
            {
                Providers.registerProvider<T>();
                Providers.registerProvider<RocketBuiltinCommandProvider>();
                Providers.registerProvider<RocketBuiltinTranslationProvider>();
                Providers.registerProvider<RocketBuiltinPermissionsProvider>();

                Providers.Load();

                Translations.RegisterDefaultTranslations(defaultTranslation);
                Translations.RegisterDefaultTranslations(Implementation.DefaultTranslation);

                gameObject.TryAddComponent<TaskDispatcher>();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }
    }
}