using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.API.Plugins;
using Rocket.Collections;
using Rocket.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rocket.API.Commands
{
    public class RocketCommandList<T> : IEnumerable<RegisteredRocketCommand<T>> where T : IRocketPlugin
    {
        private CooldownList<RegisteredRocketCommand<T>> cooldown;
        private List<RegisteredRocketCommand<T>> commands = new List<RegisteredRocketCommand<T>>();
        public ReadOnlyCollection<RegisteredRocketCommand<T>> Commands { get; internal set; }

        private IRocketPluginManager<T> manager;

        public RocketCommandList(IRocketPluginManager<T> manager){
            this.manager = manager;
            Commands = commands.AsReadOnly();
            
        }

        public delegate void ExecuteCommand(IRocketPlayer player, IRocketCommand<T> command, ref bool cancel);
        public event ExecuteCommand OnExecuteCommand;

        public IRocketCommand<T> GetCommand(IRocketCommand<T> plugin)
        {
            return GetCommand(plugin.Name);
        }

        public IRocketCommand<T> GetCommand(string name)
        {
            return commands.Where(c => c.Name == name).FirstOrDefault();
        }

        public IEnumerator<RegisteredRocketCommand<T>> GetEnumerator()
        {
            return commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


        public void Add(IRocketCommand<T> command, string alias = null, ECommandPriority priority = ECommandPriority.Normal)
        {
            string name = command.Name;
            if (alias != null) name = alias;

            Add(new Commands.RegisteredRocketCommand<T>(manager, name, command));
            this.GetLogger().Info("[registered] /" + name + " (" + command.Identifier + ")");
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
                IRocketCommand<T> rocketCommand = GetCommand(name);
                if (rocketCommand != null)
                {
                    if (rocketCommand.AllowedCaller == AllowedCaller.Player && player is ConsolePlayer)
                    {
                        this.GetLogger().Warn("This command can't be called from console");
                        return false;
                    }
                    if (rocketCommand.AllowedCaller == AllowedCaller.Console && !(player is ConsolePlayer))
                    {
                        this.GetLogger().Warn("This command can only be called from console");
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
                                    this.GetLogger().Error(ex);
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
                                this.GetLogger().Warn(ex.Message);
                            }
                            catch (WrongUsageOfCommandException ex)
                            {
                                this.GetLogger().Info(ex.Message);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.GetLogger().Error("An error occured while executing " + rocketCommand.Name + " [" + String.Join(", ", parameters) + "]", ex);
                    }
                    return true;
                }
            }

            return false;
        }

    }
}
