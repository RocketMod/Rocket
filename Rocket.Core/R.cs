using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Reflection;
using Rocket.API.Event;
using Rocket.API.Event.Command;
using Rocket.API.Extensions;
using Rocket.API.Player;
using Logger = Rocket.API.Logging.Logger;
using Rocket.Plugins.Native;
using Rocket.Core.Commands;
using Rocket.API.Serialisation;
using Rocket.API.Providers;
using Rocket.API.Utils;
using Rocket.Core.Assets;
using Rocket.Core.Providers.Logging;
using Rocket.Core.Providers.Permissions;
using Rocket.Core.Providers.Remoting.RCON;
using Rocket.Core.Providers.Remoting.RPC;
using Rocket.Core.Utils.Debugging;
using Rocket.API.Providers.Implementation;
using Rocket.API.Providers.Commands;
using Rocket.API.Providers.Remoting;
using Rocket.API.Providers.Permissions;
using Rocket.API.Providers.Translations;
using Rocket.API.Providers.Configuration;
using Rocket.API.Providers.Player;

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

        private static readonly List<Type> BuiltinProviders = new List<Type>()
        {
            typeof(RocketBuiltinCommandProvider),
            typeof(RocketBuiltinPermissionsProvider),
            typeof(Log4NetLoggingProvider)
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



        public static IRocketImplementationProvider Implementation => GetProvider<IRocketImplementationProvider>();
        public static IRocketTranslationDataProvider Translations => GetProvider<IRocketTranslationDataProvider>();
        public static RocketLoggingProviderProxy Logger { get; set; } = new RocketLoggingProviderProxy();

        /* TODO */
        public static RocketPluginProviderProxy PluginProviders { get; set; } = new RocketPluginProviderProxy();
        public static IRocketPermissionsDataProvider Permissions => GetProvider<IRocketPermissionsDataProvider>();
      




        private static readonly GameObject gameObject = new GameObject("Rocket");

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

        public static bool RunOnProvider<T>(Action<T> action)
        {
            var providers = GetProviders<T>();
            if (providers.Count == 0)
                return false;

            foreach(var provider in providers)
                action.Invoke(provider);

            return true;
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
                R.Logger.Fatal(ex);
            }
        }

        public static bool Execute(IRocketPlayer caller, string commandString)
        {
            R.Logger.Debug("EXECUTE:"+commandString);
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

                R.Logger.Debug("NAME:"+name);

                foreach (IRocketPluginProvider p in PluginProviders)
                {
                    IRocketCommand c = p.CommandProvider.GetCommand(name);
                    if (c != null) commands.Add(c);
                }

                //TODO: Figure a way to prioritise commands

                if (commands.Count <= 0)
                {
                    R.Logger.Info("Command not found");
                    return false;
                }

                IRocketCommand command = commands[0];

                PreCommandExecuteEvent @event = new PreCommandExecuteEvent(caller, command, parameters);
                EventManager.Instance.CallEvent(@event);

                if (!@event.IsCancelled)
                {
                    try
                    {
                        command.Execute(caller, parameters);
                        R.Logger.Debug("EXECUTED");
                        return true;
                    }
                    catch (NoPermissionsForCommandException ex)
                    {
                        R.Logger.Warn(ex);
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
            }
            catch (Exception ex)
            {
                R.Logger.Error("An error occured while executing " + name + " [" + String.Join(", ", parameters) + "]", ex);
            }
            return false;
        }

        static void Bootstrap<T>() where T : IRocketImplementationProvider
        {
            R.Logger.Info("####################################################################################");
            R.Logger.Info("Starting RocketMod " + R.Version);
            R.Logger.Info("####################################################################################");

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
                foreach (Type t in implementation.Providers)
                    registerProvider(t);

                gameObject.TryAddComponent<TaskDispatcher>();

                registerProvider<RocketBuiltinCommandProvider>();
                registerProvider<RocketBuiltinPermissionsProvider>();
                
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