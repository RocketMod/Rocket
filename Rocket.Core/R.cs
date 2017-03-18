using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Core.Extensions;
using Rocket.Core.Permissions;
using Rocket.Core.Tasks;
using Rocket.Core.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Reflection;
using Logger = Rocket.API.Logging.Logger;
using Rocket.Plugins.Native;
using Rocket.Core.RCON;
using Rocket.Core.Commands;
using Rocket.API.Serialisation;
using Rocket.API.Providers;
using Rocket.Core.Providers;
using Rocket.Core.Utils.Debugging;

namespace Rocket.Core
{

    public static class R
    {
        private static readonly List<ProviderRegistration> availableProviderTypes = new List<ProviderRegistration>()
        {
            new ProviderRegistration(typeof(IRocketImplementationProvider),false),
            new ProviderRegistration(typeof(IRocketCommandProvider),true),
            new ProviderRegistration(typeof(IRocketPluginProvider),true),
            new ProviderRegistration(typeof(IRocketRemotingProvider),true),

            new ProviderRegistration(typeof(IRocketPermissionsDataProvider),false),
            new ProviderRegistration(typeof(IRocketTranslationDataProvider),false),
            new ProviderRegistration(typeof(IRocketConfigurationDataProvider),false),
            new ProviderRegistration(typeof(IRocketPlayerDataProvider),false)
        };


        private static GameObject rocketGameObject = new GameObject("Rocket");
        private static List<ProviderRegistration> providers  = new List<ProviderRegistration>();

        private static ProviderRegistration getProviderInterface(Type provider)
        {
            ProviderRegistration currentProviderType = null;
            Type[] currentProviderTypes = provider.GetType().GetInterfaces();
            foreach (ProviderRegistration registration in availableProviderTypes)
            {
                if (currentProviderTypes.Contains(registration.Type))
                {
                    currentProviderType = registration;
                    break;
                }
            }
            if (currentProviderType == null) throw new ArgumentException("Provider has no known interface");
            if (!(provider.GetType().IsAssignableFrom(typeof(RocketProviderBase)))) throw new ArgumentException("Provider does not implement RocketProviderBase");
            return currentProviderType;
        }

        private static T registerProvider<T>() where T : RocketProviderBase
        {
            return (T)registerProvider(typeof(T));
        }

        private static RocketProviderBase registerProvider(Type provider) 
        {
            ProviderRegistration currentProviderType = getProviderInterface(provider);

            if (!currentProviderType.AllowMultipleInstances)
            {
                providers.Where(p => p.Type == currentProviderType.Type).All(p => { p.Enabled = false; p.Implementation.enabled = false; return true; });
            }
            ProviderRegistration result = new ProviderRegistration(currentProviderType, (RocketProviderBase)rocketGameObject.AddComponent(provider));
            providers.Add(result);
            return result.Implementation;
        }

        public static RocketProviderBase GetProvider(Type providerType) 
        {
            return GetProviders(providerType).FirstOrDefault();
        }

        public static List<RocketProviderBase> GetProviders(Type providerType)
        {
            if(!providerType.IsInterface) throw new ArgumentException("The given type is no interface");
            if(availableProviderTypes.Where(t => t.Type == providerType).FirstOrDefault() == null) throw new ArgumentException("The given type is not a known provider interface");
            return providers.Where(p => p.Type == providerType && p.Enabled).Select(p => p.Implementation).ToList();
        }

        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static TranslationList DefaultTranslation
        {
            get
            {
                return new TranslationList()
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
            }
        }

        public static void Reload()
        {
            try
            {
                foreach (ProviderRegistration provider in providers)
                {
                    if (provider.Implementation.GetType().IsAssignableFrom(typeof(IRocketProviderBase)))
                    {
                        ((IRocketProviderBase)provider.Implementation).Unload();
                    }
                }
                foreach (ProviderRegistration provider in providers)
                {
                    if (provider.Implementation.GetType().IsAssignableFrom(typeof(IRocketProviderBase)))
                    {
                        ((IRocketProviderBase)provider.Implementation).Load();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        public static bool Execute(IRocketPlayer caller, string commandString)
        {
            Logger.Debug("EXECUTE:"+commandString);
            string name = "";
            string[] parameters = new string[0];
            try
            {
                commandString = commandString.TrimStart('/');
                string[] commandParts = Regex.Matches(commandString, @"[\""](.+?)[\""]|([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Cast<Match>().Select(m => m.Value.Trim('"').Trim()).ToArray();

                if (commandParts.Length != 0)
                {
                    name = commandParts[0];
                    parameters = commandParts.Skip(1).ToArray();
                }
                if (caller == null) caller = new ConsolePlayer();

                List<IRocketCommand> commands = new List<IRocketCommand>();

                Logger.Debug("NAME:"+name);

                foreach (IRocketPluginProvider p in PluginManagers)
                {
                    IRocketCommand c = p.Commands.GetCommand(name);
                    if (c != null) commands.Add(c);
                }

                //TODO: Figure a way to prioritise commands

                if (commands.Count > 0)
                {
                    IRocketCommand command = commands[0];

                    bool cancelCommand = false;
                    if (OnCommandExecute != null)
                    {
                        foreach (var handler in OnCommandExecute.GetInvocationList().Cast<RockedCommandExecute>())
                        {
                            try
                            {
                                handler(caller, command, ref cancelCommand);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        }
                    }
                    if (!cancelCommand)
                    {
                        try
                        {
                            command.Execute(caller, parameters);
                            Logger.Debug("EXECUTED");
                            return true;
                        }
                        catch (NoPermissionsForCommandException ex)
                        {
                            Logger.Warn(ex);
                        }
                        catch (WrongUsageOfCommandException)
                        {
                            //
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }else
                {
                    Logger.Info("Command not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while executing " + name + " [" + String.Join(", ", parameters) + "]", ex);
            }
            return false;
        }

        static void Bootstrap<T>() where T : IRocketImplementationProvider
        {
            Logger.Info("####################################################################################");
            Logger.Info("Starting RocketMod " + R.Version);
            Logger.Info("####################################################################################");

            #if DEBUG
                gameObject.TryAddComponent<Debugger>();
            #else
                Initialize<T>();
            #endif
        }

        private static void initialize<T>() where T : IRocketImplementationProvider
        {
            try
            {

                IRocketImplementationProvider implementation = (IRocketImplementationProvider)registerProvider(typeof(T));
                foreach (Type t in implementation.GetProviders()) registerProvider(t);

                registerProvider<RocketBuiltinCommandProvider>();
                registerProvider<RocketBuiltinPermissionsProvider>();



                NativeRocketPluginProvider nativePlugins = registerProvider<NativeRocketPluginProvider>();



                Settings = new XMLFileAsset<RocketSettings>(API.Environment.SettingsFile);
                API.Environment.LanguageCode = Settings.Instance.LanguageCode;
                TranslationList defaultTranslations = new TranslationList();
                defaultTranslations.AddRange(new RocketTranslations());
                Translation = new XMLFileAsset<TranslationList>(String.Format(API.Environment.TranslationFile, Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(PropertyListEntry) }, defaultTranslations);
                Translation.AddUnknownEntries(defaultTranslations);

                Permissions = gameObject.TryAddComponent<RocketBuiltinPermissionsProvider>();
                gameObject.TryAddComponent<TaskDispatcher>();

                NativeRocketPluginProvider nativeRocketPluginManager = gameObject.TryAddComponent<NativeRocketPluginProvider>();
                nativeRocketPluginManager.AddCommands(new List<IRocketCommand>()
                {
                });
                nativeRocketPluginManager.AddCommands(Implementation.GetAllCommands());
                nativeRocketPluginManager.Load(API.Environment.PluginsDirectory, Settings.Instance.LanguageCode, API.Environment.LibrariesDirectory);
                nativeRocketPluginManager.Commands.Persist();


                PluginManagers.Add(nativeRocketPluginManager);

                try
                {
                    if (Settings.Instance.RPC.Enabled)
                        new RocketServiceHost(Settings.Instance.RPC.Port);
                    if (Settings.Instance.RCON.Enabled)
                        gameObject.TryAddComponent<RCONServer>();

                }
                catch (Exception e)
                {
                    Logger.Error("Uh ohh.."+e);
                }

                Implementation.OnInitialized += () =>
                {
                    if (Settings.Instance.MaxFrames < 10 && Settings.Instance.MaxFrames != -1) Settings.Instance.MaxFrames = 10;
                    Application.targetFrameRate = Settings.Instance.MaxFrames;
                };
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

    }
}