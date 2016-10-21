using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Collections;
using Rocket.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Commands
{
    public class RegisteredRocketCommand : IRocketCommand, ICooldownItem
    {
        public IRocketCommand Command;
        private string name;

        public RegisteredRocketCommand(IRocketPluginManager manager, string name, IRocketCommand command)
        {
            this.manager = manager;
            this.name = name;
            Command = command;
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

        private IRocketPluginManager manager;

        public event CooldownExpired OnCooldownExpired;

        public IRocketPluginManager Manager
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
            OnCooldownExpired.TryInvoke();
            throw new NotImplementedException();
        }
    }

}
