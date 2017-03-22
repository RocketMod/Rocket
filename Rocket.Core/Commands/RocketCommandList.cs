using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Rocket.API.Commands;
using Rocket.API.Event;
using Rocket.API.Event.Command;
using Rocket.API.Exceptions;

using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Plugins;
using Rocket.API.Serialisation;
using Rocket.Core;

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

            foreach (string alias in command.Aliases)
            {
                commands.Add(new RegisteredRocketCommand(manager, alias, command));
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
                        return false;
                    }
                    if (rocketCommand.AllowedCaller == AllowedCaller.Console && !(player is ConsolePlayer))
                    {
                        return false;
                    }
                    try
                    {
                        ExecuteCommandEvent @event = new ExecuteCommandEvent(player, rocketCommand, parameters);
                        EventManager.Instance.CallEvent(@event);

                        if (@event.IsCancelled)
                        {
                            return false;
                        }

                        try
                        {
                            rocketCommand.Execute(player, parameters);
                        }
                        catch (NoPermissionsForCommandException ex)
                        {
                            R.Logger.Warn(ex.Message);
                        }
                        catch (WrongUsageOfCommandException ex)
                        {
                            R.Logger.Info(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        R.Logger.Error("An error occured while executing " + rocketCommand.Name + " [" + String.Join(", ", parameters) + "]", ex);
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
