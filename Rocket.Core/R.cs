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

namespace Rocket.Core
{
    public delegate void RockedCommandExecute(IRocketPlayer player, IRocketCommand command, ref bool cancel);

    public class ProviderRegistration
    {
        public ProviderRegistration(Type type, Type baseClass, bool allowMultipleInstances)
        {
            Type = type;
            BaseClass = baseClass;
            AllowMultipleInstances = allowMultipleInstances;
        }

        public ProviderRegistration(ProviderRegistration registration,object implementation)
        {
            Type = registration.Type;
            BaseClass = registration.BaseClass;
            AllowMultipleInstances = registration.AllowMultipleInstances;
            Implementation = implementation;
        }
        public Type Type { get; private set; }
        public Type BaseClass { get; private set; }
        public bool AllowMultipleInstances { get; private set; }
        public object Implementation { get; set; } = null;
    }


    public class R : MonoBehaviour
    {
        private static readonly List<ProviderRegistration> availableProviderTypes = new List<ProviderRegistration>()
        {
            new ProviderRegistration(typeof(IRocketImplementationProvider),null,false),
            new ProviderRegistration(typeof(IRocketCommandProvider),null,true),
            new ProviderRegistration(typeof(IRocketPluginProvider),typeof(RocketProviderBase),true),
            new ProviderRegistration(typeof(IRocketRemotingProvider),typeof(RocketProviderBase),true),

            new ProviderRegistration(typeof(IRocketPermissionsDataProvider),typeof(RocketDataProviderBase),false),
            new ProviderRegistration(typeof(IRocketTranslationDataProvider),typeof(RocketDataProviderBase),false),
            new ProviderRegistration(typeof(IRocketConfigurationDataProvider),typeof(RocketDataProviderBase),false),
            new ProviderRegistration(typeof(IRocketPlayerDataProvider),typeof(RocketDataProviderBase),false)
        };

        

        private static List<ProviderRegistration> providers  = new List<ProviderRegistration>();

        internal static void RegisterProvider<T>(object provider)
        {
            Logger.Info("Registering " +provider.ToString());
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
            if (currentProviderType == null) throw new Exception("Provider has no known interface");

            if(currentProviderType.BaseClass != null)
                if (!(provider.GetType().IsAssignableFrom(currentProviderType.BaseClass))) throw new Exception("Provider does not implement valid base class");

            if (!currentProviderType.AllowMultipleInstances)
            {
                providers.RemoveAll(p => p.Type == currentProviderType.Type);
            }
            providers.Add(new ProviderRegistration(currentProviderType,provider));
        }

        public static T GetProvider<T>() 
        {
            return GetProviders<T>().FirstOrDefault();
        }

        public static List<T> GetProviders<T>()
        {
            return providers.Where(provider => provider.Type == typeof(T) && provider.Implementation is T).Select(provider => (T)provider.Implementation).ToList();
        }

        internal static R instance;
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
     
     
        public static void Reload()
        {
            try
            {
                foreach (ProviderRegistration provider in providers)
                {
                    if (provider.Implementation.GetType().IsAssignableFrom(typeof(RocketProviderBase)))
                    {
                        ((RocketProviderBase)provider.Implementation).Unload();
                    }
                }
                foreach (ProviderRegistration provider in providers)
                {
                    if (provider.Implementation.GetType().IsAssignableFrom(typeof(RocketProviderBase)))
                    {
                        ((RocketProviderBase)provider.Implementation).Load();
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
      
        private void Awake()
        {
            instance = this;
            API.Environment.Initialize();
            Logger.Initialize(API.Environment.LogConfigurationFile);
            Logger.Info("####################################################################################");
            Logger.Info("Starting RocketMod " + R.Version + ";
            Logger.Info("####################################################################################");


            #if DEBUG
                gameObject.TryAddComponent<Debugger>();
            #else
                Initialize();
            #endif
        }

        internal void Initialize()
        {
            try
            {
                Settings = new XMLFileAsset<RocketSettings>(API.Environment.SettingsFile);
                API.Environment.LanguageCode = Settings.Instance.LanguageCode;
                TranslationList defaultTranslations = new TranslationList();
                defaultTranslations.AddRange(new RocketTranslations());
                Translation = new XMLFileAsset<TranslationList>(String.Format(API.Environment.TranslationFile, Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(PropertyListEntry) }, defaultTranslations);
                Translation.AddUnknownEntries(defaultTranslations);

                Permissions = gameObject.TryAddComponent<RocketPermissionsProvider>();
                gameObject.TryAddComponent<TaskDispatcher>();

                NativeRocketPluginProvider nativeRocketPluginManager = gameObject.TryAddComponent<NativeRocketPluginProvider>();
                nativeRocketPluginManager.AddCommands(new List<IRocketCommand>()
                {
                    new CommandExit(),
                    new CommandHelp(),
                    new CommandP(),
                    new CommandRocket()
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