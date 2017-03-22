using Rocket.API.Collections;
using System;
using System.ComponentModel;
using UnityEngine;
using System.Reflection;
using Rocket.API.Extensions;
using Rocket.API.Providers.Configuration;
using Rocket.API.Utils;
using Rocket.API.Providers.Implementation;
using Rocket.API.Providers.Logging;
using Rocket.API.Providers.Permissions;
using Rocket.API.Providers.Plugins;
using Rocket.API.Providers.Remoting;
using Rocket.API.Providers.Translations;
using Rocket.Core.Managers;

namespace Rocket.Core
{
    public static class R
    {
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static TranslationList DefaultTranslation => new TranslationList
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

        public static IRocketImplementationProvider Implementation => Providers.GetProvider<IRocketImplementationProvider>();
        public static IRocketTranslationDataProvider Translations => Providers.GetProvider<IRocketTranslationDataProvider>();
        public static IRocketPermissionsDataProvider Permissions => Providers.GetProvider<IRocketPermissionsDataProvider>();
        public static IRocketLoggingProvider Logger => Providers.GetProvider<IRocketLoggingProvider>();
        public static IRocketPluginProvider Plugins => Providers.GetProvider<IRocketPluginProvider>();
        public static IRocketConfigurationDataProvider Configuration => Providers.GetProvider<IRocketConfigurationDataProvider>();
        public static IRocketRemotingProvider Remoting => Providers.GetProvider<IRocketRemotingProvider>();

        public static void Reload()
        {
            try {
                Providers.Reload();
            }
            catch (Exception ex)
            {
                R.Logger.Fatal(ex);
            }
        }

        private static GameObject gameObject = new GameObject("Rocket");

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Bootstrap<T>() where T : IRocketImplementationProvider
        {
            R.Logger.Info("####################################################################################");
            R.Logger.Info("Starting RocketMod " + R.Version);
            R.Logger.Info("####################################################################################");

            try
            {
                gameObject.TryAddComponent<TaskDispatcher>();

                //NativeRocketPluginProvider nativePlugins = registerProvider<NativeRocketPluginProvider>();
                //nativePlugins.AddCommands(new List<IRocketCommand>
                //{
                //    //todo
                //});

                //nativePlugins.Load(API.Environment.PluginsDirectory, Settings.Instance.LanguageCode, API.Environment.LibrariesDirectory);

                //Settings = new XMLFileAsset<RocketSettings>(API.Environment.SettingsFile);
                //TranslationList defaultTranslations = new TranslationList();
                ////defaultTranslations.AddRange(new RocketTranslations());

                //RunOnProvider<IRocketTranslationDataProvider>(provider =>
                //{
                //    provider.RegisterDefaultTranslations(defaultTranslations);
                //});

           
                ////nativeRocketPluginManager.CommandProvider.Persist();

                //try
                //{
                //    if (Settings.Instance.RPC.Enabled)
                //        new RocketServiceHost(Settings.Instance.RPC.Port);
                //    if (Settings.Instance.RCON.Enabled)
                //        gameObject.TryAddComponent<RCONServer>();

                //}
                //catch (Exception e)
                //{
                //    R.Logger.Error("Failed to start RPC / RCON", e);
                //}

                //Implementation.OnInitialized += () =>
                //{
                //    if (Settings.Instance.MaxFrames < 10 && Settings.Instance.MaxFrames != -1) Settings.Instance.MaxFrames = 10;
                //    Application.targetFrameRate = Settings.Instance.MaxFrames;
                //};
            }
            catch (Exception ex)
            {
                R.Logger.Fatal(ex);
            }
        }

    }
}