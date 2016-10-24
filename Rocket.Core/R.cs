using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.API.Extensions;
using Rocket.API.Permissions;
using Rocket.API.Plugins;
using Rocket.Core.Extensions;
using Rocket.Core.Permissions;
using Rocket.Core.Serialization;
using Rocket.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Reflection;
using Rocket.Core.IPC;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Core
{
    public delegate void RockedInitialized();
    public delegate void RockedCommandExecute(IRocketPlayer player, IRocketCommand command, ref bool cancel);

    public class R : MonoBehaviour
    {
        #region Events
            public static event RockedCommandExecute OnCommandExecute;
            public static event RockedInitialized OnInitialized;
        #endregion

        #region Static Properties
        public static R Instance { get; private set; }
        public static IRocketImplementation Implementation { get; private set; }
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        #endregion

        #region Static Methods
        public static string Translate(string translationKey, params object[] placeholder)
        {
            return Instance.Translation.Instance.Translate(translationKey, placeholder);
        }
        #endregion

        #region Properties
        public IRocketPermissionsProvider Permissions { get; set; }
        public RocketServiceHost IPC { get; private set; }
        public XMLFileAsset<RocketSettings> Settings { get; private set; }
        public XMLFileAsset<TranslationList> Translation { get; private set; }
        public List<IRocketPluginManager> PluginManagers { get; private set; } = new List<IRocketPluginManager>();
        #endregion

        public ReadOnlyCollection<IRocketPlugin> GetAllPlugins()
        {
            List<IRocketPlugin> plugins = new List<IRocketPlugin>();
            foreach (IRocketPluginManager m in PluginManagers)
            {
                plugins.AddRange(m.GetPlugins());
            }
            return plugins.AsReadOnly();
        }
        public ReadOnlyCollection<IRocketCommand> GetAllCommands()
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

        public IRocketCommand GetCommand(string name)
        {
            IRocketCommand commands = null;
            foreach (IRocketPluginManager m in PluginManagers)
            {
                commands = m.Commands.GetCommand(name);
            }
            return commands;
        }

        private void Awake()
        {
            Instance = this;
            Implementation = (IRocketImplementation)GetComponent(typeof(IRocketImplementation));
            Environment.Initialize();

            #if FALSE//DEBUG
                gameObject.TryAddComponent<Debugger>();
            #else
                Initialize();
            #endif
        }

        private void Initialize()
        {
            try
            {

                Settings = new XMLFileAsset<RocketSettings>(Environment.SettingsFile);

                RocketTranslations defaultTranslations = new RocketTranslations();
                Translation = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, defaultTranslations);
                Translation.AddUnknownEntries(defaultTranslations);

                Permissions = gameObject.TryAddComponent<RocketPermissionsManager>();
                gameObject.TryAddComponent<TaskDispatcher>();

                //PluginManagers.Add(gameObject.TryAddComponent<NativeRocketPluginManager>());

                try
                {
                    if (Settings.Instance.RPC.Enabled)
                        IPC = new RocketServiceHost(Settings.Instance.RPC.Port);
                }
                catch (Exception t)
                {
                    Logger.Error(t);
                }

                Implementation.OnInitialized += () =>
                {
                    if (Settings.Instance.MaxFrames < 10 && Settings.Instance.MaxFrames != -1) Settings.Instance.MaxFrames = 10;
                    Application.targetFrameRate = Settings.Instance.MaxFrames;
                };
                OnInitialized.TryInvoke();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        public void Reload()
        {
            Settings.Load();
            Translation.Load();
            Permissions.Reload();
            PluginManagers.ForEach(p => p.Reload());
            Implementation.Reload();
        }
        
        public bool Execute(IRocketPlayer caller, string commandString)
        {
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


                foreach (IRocketPluginManager p in Instance.PluginManagers)
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
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while executing " + name + " [" + String.Join(", ", parameters) + "]", ex);
            }
            return false;
        }
    }
}