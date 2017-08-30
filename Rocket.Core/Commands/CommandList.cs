using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.Serialization;
using Rocket.API.Providers;
using Rocket.API.Plugins;

namespace Rocket.Core.Commands
{
    [Serializable]
    public class CommandList : IEnumerable<RegisteredRocketCommand>, IDefaultable
    {
        private List<RegisteredRocketCommand> commands = new List<RegisteredRocketCommand>();
        public ReadOnlyCollection<RegisteredRocketCommand> Commands { get; internal set; }

        private IPluginProvider manager;

        public CommandList()
        {
            Commands = commands.AsReadOnly();
        }

        public CommandList(IPluginProvider manager){
            this.manager = manager;
            Commands = commands.AsReadOnly();
        }

        public delegate void ExecuteCommand(IPlayer player, ICommand command, ref bool cancel);

        public RegisteredRocketCommand GetCommand(IPlugin plugin)
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

        public void Add(ICommand command)
        {
            string name = manager.GetType().Assembly.GetName().Name;
            string identifier = name+"."+command.GetType().Assembly.GetName().Name+"."+command.Name;
            commands.Add(new RegisteredRocketCommand(manager, command.Name, command));

            foreach (string alias in command.Aliases)
            {
                commands.Add(new RegisteredRocketCommand(manager, alias, command));
            }
        }

        public void AddRange(IEnumerable<ICommand> commands)
        {
            foreach (ICommand command in commands)
            {
                Add(command);
            }
        }

        public void LoadDefaults()
        {
            Commands = new ReadOnlyCollection<RegisteredRocketCommand>(new List<RegisteredRocketCommand>());
        }
    }
}
