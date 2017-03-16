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

namespace Rocket.Core
{
    public delegate void RockedCommandExecute(IRocketPlayer player, IRocketCommand command, ref bool cancel);

    public class R : MonoBehaviour
    {
        #region Events
            public static event RockedCommandExecute OnCommandExecute;
        #endregion

        #region Static Properties
        internal static R instance;
        public static IRocketImplementation Implementation { get; private set; }
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static IRocketPermissionsProvider Permissions { get; set; }
        public static XMLFileAsset<RocketSettings> Settings { get; private set; }
        public static XMLFileAsset<TranslationList> Translation { get; private set; }
        public static List<IRocketPluginManager> PluginManagers { get; private set; } = new List<IRocketPluginManager>();
        #endregion

        #region Static Methods
        public static string Translate(string translationKey, params object[] placeholder)
        {
            return Translation.Instance.Translate(translationKey, placeholder);
        }
        
        public static ReadOnlyCollection<IRocketPlugin> GetAllPlugins()
        {
            List<IRocketPlugin> plugins = new List<IRocketPlugin>();
            foreach (IRocketPluginManager m in PluginManagers)
            {
                plugins.AddRange(m.GetPlugins());
            }
            return plugins.AsReadOnly();
        }

        public static ReadOnlyCollection<IRocketCommand> GetAllCommands()
        {
            List<IRocketCommand> commands = new List<IRocketCommand>();
            foreach (IRocketPluginManager m in PluginManagers)
            {
                foreach(IRocketCommand c in m.Commands.Commands)
                {
                    commands.Add(c);
                }
            }
            return commands.AsReadOnly();
        }

        public static IRocketCommand GetCommand(string name)
        {
            IRocketCommand commands = null;
            foreach (IRocketPluginManager m in PluginManagers)
            {
                commands = m.Commands.GetCommand(name);
            }
            return commands;
        }

        public static void Reload()
        {
            try
            {
                Settings.Load();
                Translation.Load();
                Permissions.Reload();
                PluginManagers.ForEach(p => p.Reload());
                Implementation.Reload();
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

                foreach (IRocketPluginManager p in PluginManagers)
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
        #endregion

        private void Awake()
        {
            instance = this;
            Implementation = (IRocketImplementation)GetComponent(typeof(IRocketImplementation));
            API.Environment.Initialize();
            Logger.Initialize(API.Environment.LogConfigurationFile);
            Logger.Info("####################################################################################");
            Logger.Info("Starting RocketMod " + R.Version + " for " + R.Implementation?.Name + " on instance " + R.Implementation?.InstanceName);
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

                NativeRocketPluginManager nativeRocketPluginManager = gameObject.TryAddComponent<NativeRocketPluginManager>();
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