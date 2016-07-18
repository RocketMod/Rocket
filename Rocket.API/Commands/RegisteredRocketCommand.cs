using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Commands
{
    public class RegisteredRocketCommand<T> : IRocketCommand<T>, ICooldownItem where T : IRocketPlugin
    {
        public Type Type;
        public IRocketCommand<T> Command;
        private string name;

        public RegisteredRocketCommand(IRocketPluginManager<T>manager, string name, IRocketCommand<T> command)
        {
            this.manager = manager;
            this.name = name;
            Command = command;
            Type = command.GetCommandType();
        }

        public List<string> Aliases
        {
            get
            {
                return Command.Aliases;
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return Command.AllowedCaller;
            }
        }

        public string Help
        {
            get
            {
                return Command.Help;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Identifier  {
            get { return name; }
        }

        public List<string> Permissions
        {
            get
            {
                return Command.Permissions;
            }
        }

        public string Syntax
        {
            get
            {
                return Command.Syntax;
            }
        }

        private IRocketPluginManager<T> manager;

        public event CooldownExpired OnCooldownExpired;

        public IRocketPluginManager<T> Manager
        {
            get
            {
                return manager;
            }
        }

        public double Cooldown
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private DateTime started;

        public DateTime Started
        {
            get { return started; }
            set { started = value; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Command.Execute(caller, command);
        }

        public void Expire()
        {
            throw new NotImplementedException();
        }
    }

}
