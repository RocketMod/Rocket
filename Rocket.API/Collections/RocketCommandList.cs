using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Rocket.API.Assets;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.API.Providers;

namespace Rocket.API.Collections
{
    [Serializable]
    public class RocketCommandList : IEnumerable<RegisteredRocketCommand>, IDefaultable
    {
        private List<RegisteredRocketCommand> commands = new List<RegisteredRocketCommand>();
        public ReadOnlyCollection<RegisteredRocketCommand> Commands { get; internal set; }

        private IRocketPluginProvider manager;

        public RocketCommandList()
        {
            Commands = commands.AsReadOnly();
        }

        public RocketCommandList(IRocketPluginProvider manager){
            this.manager = manager;
            Commands = commands.AsReadOnly();
        }

        public delegate void ExecuteCommand(IRocketPlayer player, IRocketCommand command, ref bool cancel);
        public event ExecuteCommand OnExecuteCommand;

        public RegisteredRocketCommand GetCommand(IRocketPlugin plugin)
        {
            return GetCommand(plugin.Name);
        }

        public RegisteredRocketCommand GetCommand(string name)
        {
            return commands.FirstOrDefault(c => c.Name.ToLower() == name.ToLower() && c.Enabled);
        }

        public IEnumerator<RegisteredRocketCommand> GetEnumerator()
        {
            return commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Commands.GetEnumerator();
        }

        public void Add(RegisteredRocketCommand command)
        {
            commands.Add(command);
        }

        public void Add(IRocketCommand command)
        {
            string name = manager.GetType().Assembly.GetName().Name;
            string identifier = name+"."+command.GetType().Assembly.GetName().Name+"."+command.Name;
            commands.Add(new RegisteredRocketCommand(manager, command.Name, command));
            Logger.Info("[registered] /" + command.Name + " (" + identifier + ")");

            foreach (string alias in command.Aliases)
            {
                commands.Add(new RegisteredRocketCommand(manager, alias, command));
                Logger.Info("[registered alias] /" + alias + " (" +identifier + ")");
            }
        }

        public void AddRange(IEnumerable<IRocketCommand> commands)
        {
            foreach (IRocketCommand command in commands)
            {
                Add(command);
            }
        }

        public bool Execute(IRocketPlayer player, string command)
        {
            command = command.TrimStart('/');
            string[] commandParts = Regex.Matches(command, @"[\""](.+?)[\""]|([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Cast<Match>().Select(m => m.Value.Trim('"').Trim()).ToArray();

            if (commandParts.Length != 0)
            {
                string name = commandParts[0];
                string[] parameters = commandParts.Skip(1).ToArray();
                if (player == null) player = new ConsolePlayer();
                IRocketCommand rocketCommand = GetCommand(name);
                if (rocketCommand != null)
                {
                    if (rocketCommand.AllowedCaller == AllowedCaller.Player && player is ConsolePlayer)
                    {
                        Logger.Warn("This command can't be called from console");
                        return false;
                    }
                    if (rocketCommand.AllowedCaller == AllowedCaller.Console && !(player is ConsolePlayer))
                    {
                        Logger.Warn("This command can only be called from console");
                        return false;
                    }
                    try
                    {
                        bool cancelCommand = false;
                        if (OnExecuteCommand != null)
                        {
                            foreach (var handler in OnExecuteCommand.GetInvocationList().Cast<ExecuteCommand>())
                            {
                                try
                                {
                                    handler(player, rocketCommand, ref cancelCommand);
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
                                rocketCommand.Execute(player, parameters);
                            }
                            catch (NoPermissionsForCommandException ex)
                            {
                                Logger.Warn(ex.Message);
                            }
                            catch (WrongUsageOfCommandException ex)
                            {
                                Logger.Info(ex.Message);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("An error occured while executing " + rocketCommand.Name + " [" + String.Join(", ", parameters) + "]", ex);
                    }
                    return true;
                }
            }

            return false;
        }

        public void LoadDefaults()
        {
            Commands = new ReadOnlyCollection<RegisteredRocketCommand>(new List<RegisteredRocketCommand>());
        }
    }
}
