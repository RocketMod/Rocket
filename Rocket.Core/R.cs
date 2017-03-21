using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Reflection;
using Rocket.API.Player;
using Logger = Rocket.API.Logging.Logger;
using Rocket.Plugins.Native;
using Rocket.Core.Commands;
using Rocket.API.Serialisation;
using Rocket.API.Providers;
using Rocket.Core.Assets;
using Rocket.Core.Providers.Logging;
using Rocket.Core.Providers.Permissions;
using Rocket.Core.Providers.Remoting.RCON;
using Rocket.Core.Providers.Remoting.RPC;
using Rocket.Core.Utils.Debugging;
using Rocket.Core.Utils.Tasks;

namespace Rocket.Core
{

    public static class R
    {
        private static readonly List<IProviderRegistration> availableProviderTypes = new List<IProviderRegistration>
        {
            new ProviderRegistration<IRocketImplementationProvider>(false),
            new ProviderRegistration<IRocketLoggingProvider>(true),
            new ProviderRegistration<IRocketCommandProvider>(true),
            new ProviderRegistration<IRocketPluginProvider>(true),
            new ProviderRegistration<IRocketRemotingProvider>(true),

            new ProviderRegistration<IRocketPermissionsDataProvider>(true),
            new ProviderRegistration<IRocketTranslationDataProvider>(false),
            new ProviderRegistration<IRocketConfigurationDataProvider>(false),
            new ProviderRegistration<IRocketPlayerDataProvider>(false)
        };

        private static readonly List<IProviderRegistration> providers = new List<IProviderRegistration>();

        private static IProviderRegistration getProviderInterface(Type provider)
        {
            IProviderRegistration currentProviderType = null;
            Type[] currentProviderTypes = provider.GetInterfaces();
            foreach (IProviderRegistration registration in availableProviderTypes)
            {
                if (currentProviderTypes.Contains(registration.Type))
                {
                    currentProviderType = registration;
                    break;
                }
            }
            if (currentProviderType == null) throw new ArgumentException("Provider has no known interface");
            if (!(provider.IsAssignableFrom(typeof(RocketProviderBase)))) throw new ArgumentException("Provider does not implement RocketProviderBase");
            return currentProviderType;
        }

        public static IRocketTranslationDataProvider Translations => GetProvider<IRocketTranslationDataProvider>();
        public static List<IRocketPluginProvider> PluginProviders => GetProviders<IRocketPluginProvider>();
        public static IRocketImplementationProvider Implementation => GetProvider<IRocketImplementationProvider>();
        public static IRocketPermissionsDataProvider Permissions => GetProvider<IRocketPermissionsDataProvider>();

        public static List<IRocketCommand> Commands
        {
            get
            {
                var tmp = new List<IRocketCommand>();
                foreach(var pp in PluginProviders)
                    tmp.AddRange(pp.CommandProvider.Commands.ToArray());

                return tmp;
            }
        }
        private static T registerProvider<T>() where T : RocketProviderBase
        {
            return (T)registerProvider(typeof(T));
        }

        private static IRocketProviderBase registerProvider(Type provider) 
        {
            if(!provider.IsInterface) throw new Exception("The given type is not an interface");
            IProviderRegistration currentProviderType = getProviderInterface(provider);

            if (!currentProviderType.AllowMultipleInstances)
            {
                providers.Where(p => p.Type == currentProviderType.Type).All(p => { p.Enabled = false; p.Implementation.Unload(); return true; });
            }

            Type t = typeof(ProviderRegistration<>).MakeGenericType(currentProviderType.GetType().GetGenericArguments()[0]);
            IProviderRegistration result = (IProviderRegistration) Activator.CreateInstance(t, currentProviderType, (RocketProviderBase) Activator.CreateInstance(provider));
            providers.Add(result);
            return result.Implementation;
        }

        public static T GetProvider<T>() where T: IRocketProviderBase
        {
            return (T) GetProvider(typeof(T));
        }

        public static IRocketProviderBase GetProvider(Type providerType) 
        {
            var providerRegistration = availableProviderTypes.FirstOrDefault(t => t.Type == providerType);
            if (providerRegistration == null) throw new ArgumentException("The given type is not a known provider interface");
            if (providerRegistration.AllowMultipleInstances)
            {
                //use proxies to handle multiple providers
                return getProxyForProvider(providerRegistration);
            }

            return GetProviders(providerType).FirstOrDefault();
        }

        public static List<T> GetProviders<T>()
        {
            return GetProviders(typeof(T)).Cast<T>().ToList();
        }

        public static List<IRocketProviderBase> GetProviders(Type providerType)
        {
            if(!providerType.IsInterface) throw new ArgumentException("The given type is no interface");
            if (availableProviderTypes.FirstOrDefault(t => t.Type == providerType) == null) throw new ArgumentException("The given type is not a known provider interface");
            return providers.Where(p => p.Type == providerType && p.Enabled).Select(p => p.Implementation).ToList();
        }

        private static Dictionary<IProviderRegistration, RocketProviderBase> cachedProxies = new Dictionary<IProviderRegistration, RocketProviderBase>();
        private static RocketProviderBase getProxyForProvider(IProviderRegistration providerRegistration)
        {
            if (cachedProxies.ContainsKey(providerRegistration))
                return cachedProxies[providerRegistration];
            
            throw new NotImplementedException();
        }

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

        public static void Reload()
        {
            try
            {
                foreach (IProviderRegistration provider in providers)
                {
                    if (provider.Implementation.GetType().IsAssignableFrom(typeof(IRocketProviderBase)))
                    {
                        provider.Implementation.Unload();
                    }
                }
                foreach (IProviderRegistration provider in providers)
                {
                    if (provider.Implementation.GetType().IsAssignableFrom(typeof(IRocketProviderBase)))
                    {
                        provider.Implementation.Load(true);
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

                foreach (IRocketPluginProvider p in PluginProviders)
                {
                    IRocketCommand c = p.CommandProvider.GetCommand(name);
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
                new Debugger();
#else
                initialize<T>();
#endif
        }

        private static void initialize<T>() where T : IRocketImplementationProvider
        {
            try
            {

                IRocketImplementationProvider implementation = (IRocketImplementationProvider)registerProvider(typeof(T));
                foreach (Type t in implementation.Providers) registerProvider(t);

                registerProvider<RocketBuiltinCommandProvider>();
                registerProvider<RocketBuiltinPermissionsProvider>();



                NativeRocketPluginProvider nativePlugins = registerProvider<NativeRocketPluginProvider>();



                Settings = new XMLFileAsset<RocketSettings>(API.Environment.SettingsFile);
                API.Environment.LanguageCode = Settings.Instance.LanguageCode;
                TranslationList defaultTranslations = new TranslationList();
                defaultTranslations.AddRange(new RocketTranslations());
                Translation = new XMLFileAsset<TranslationList>(String.Format(API.Environment.TranslationFile, Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(PropertyListEntry) }, defaultTranslations);
                Translation.AddUnknownEntries(defaultTranslations);

                gameObject.TryAddComponent<TaskDispatcher>();

                NativeRocketPluginProvider nativeRocketPluginManager = gameObject.TryAddComponent<NativeRocketPluginProvider>();
                nativeRocketPluginManager.AddCommands(new List<IRocketCommand>()
                {
                });
                nativeRocketPluginManager.AddCommands(Implementation.GetAllCommands());
                nativeRocketPluginManager.Load(API.Environment.PluginsDirectory, Settings.Instance.LanguageCode, API.Environment.LibrariesDirectory);
                nativeRocketPluginManager.CommandProvider.Persist();


                PluginProviders.Add(nativeRocketPluginManager);

                try
                {
                    if (Settings.Instance.RPC.Enabled)
                        new RocketServiceHost(Settings.Instance.RPC.Port);
                    if (Settings.Instance.RCON.Enabled)
                        gameObject.TryAddComponent<RCONServer>();

                }
                catch (Exception e)
                {
                    Logger.Error("Uh ohh..", e);
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