using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Collections;
using Rocket.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Rocket.API.Providers;

namespace Rocket.API.Commands
{
    [Serializable]
    public class RegisteredRocketCommand : IRocketCommand
    {
        public RegisteredRocketCommand(IRocketPluginProvider manager, string name, IRocketCommand command)
        {
            Manager = manager;
            Name = name.ToLower();
            Command = command;

            AllowedCaller = command.AllowedCaller;
            Help = command.Help;
            Identifier = command.GetType().Assembly.GetName().Name+"."+name;
            Permissions = command.Permissions;
            Aliases = command.Aliases;
            Syntax = command.Syntax;
            Enabled = true;
        }

        [XmlIgnore]
        public IRocketCommand Command;

        [XmlIgnore]
        public IRocketPluginProvider Manager { get; private set; }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Command.Execute(caller, command);
        }

        public RegisteredRocketCommand() { }

        [XmlAttribute]
        public string Identifier { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public string Help { get; set; }

        [XmlIgnore]
        public AllowedCaller AllowedCaller { get; set; }

        [XmlElement]
        public string Syntax { get; set; }

        [XmlAttribute]
        public bool Enabled { get; set; }

        [XmlIgnore]
        public List<string> Permissions { get; set; }

        [XmlIgnore]
        public List<string> Aliases { get; set; }
    }

}
