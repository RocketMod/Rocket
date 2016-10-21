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
using Rocket.Core.RCON;
using Rocket.Core.Serialization;
using Rocket.Core.Tasks;
using Rocket.Plugins.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Reflection;
using Rocket.Core.IO;

namespace Rocket.Core
{

    public class R : MonoBehaviour,IRocketBase
    {
        public static R Instance { get; internal set; }
        public IRocketImplementation Implementation { get; private set; }
        public IRocketPermissionsProvider Permissions { get; set; }

        public XMLFileAsset<RocketSettings> Settings { get; private set; }
        public XMLFileAsset<TranslationList> Translation { get; private set; }

        public List<IRocketPluginManager> PluginManagers { get; private set; } = new List<IRocketPluginManager>();

        public static string Translate(string translationKey, params object[] placeholder)
        {
            return Instance.Translation.Instance.Translate(translationKey, placeholder);
        }

        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public ReadOnlyCollection<IRocketPlugin> GetPlugins()
        {
            List<IRocketPlugin> plugins = new List<IRocketPlugin>();
            foreach (IRocketPluginManager m in PluginManagers)
            {
                plugins.AddRange(m.GetPlugins());
            }
            return plugins.AsReadOnly();
        }

        public ReadOnlyCollection<IRocketCommand> GetCommands()
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

        private static readonly TranslationList defaultTranslations = new TranslationList(){
                {"rocket_join_public","{0} connected to the server" },
                {"rocket_leave_public","{0} disconnected from the server"},
                {"command_no_permission","You do not have permissions to execute this command."},
                {"command_cooldown","You have to wait {0} seconds before you can use this command again."}
        };


        public R()
        {
            R.Instance = this;
            Logging.Logger.Initialize();
            try
            {
                new PipeServer();
            }
            catch (TypeLoadException t)
            {
                Console.WriteLine(t.TypeName);
                Console.WriteLine(t);
                Console.WriteLine(t.InnerException);
                System.Threading.Thread.Sleep(40000);
            }
            Implementation = (IRocketImplementation)GetComponent(typeof(IRocketImplementation));
#if DEBUG
            gameObject.TryAddComponent<Debugger>();
#else
                Initialize();
#endif
        }

        public void AddNativeCommand(IRocketCommand command)
        {
            PluginManagers.Where(p => p.GetType() == typeof(NativeRocketPluginManager)).Cast<NativeRocketPluginManager>().First().Commands.Add(command);
        }

        internal void Initialize()
        {
            Environment.Initialize();
            try
            {
                Implementation.OnInitialized += () =>
                {
                    gameObject.TryAddComponent<TaskDispatcher>();
                    if (Settings.Instance.RCON.Enabled) gameObject.TryAddComponent<RCONServer>();
                };

                Settings = new XMLFileAsset<RocketSettings>(Environment.SettingsFile);
                Translation = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, defaultTranslations);
                defaultTranslations.AddUnknownEntries(Translation);
                Permissions = gameObject.TryAddComponent<RocketPermissionsManager>();

                PluginManagers.Add(gameObject.TryAddComponent<NativeRocketPluginManager>());

                if (Settings.Instance.MaxFrames < 10 && Settings.Instance.MaxFrames != -1) Settings.Instance.MaxFrames = 10;
                Application.targetFrameRate = Settings.Instance.MaxFrames;
                OnInitialized.TryInvoke();
            }
            catch (Exception ex)
            {
                Logging.Logger.Fatal(ex);
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

        public event RockedCommandExecute OnCommandExecute;
        public event RockedInitialized OnInitialized;

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
                                Logging.Logger.Error(ex);
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
                            Logging.Logger.Warn(ex);
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
                Logging.Logger.Error("An error occured while executing " + name + " [" + String.Join(", ", parameters) + "]", ex);
            }
            return false;
        }

    }
}