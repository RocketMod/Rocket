using Rocket.API.Providers;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API.Player;

namespace Rocket.API.Commands
{
    [Serializable]
    public class RegisteredRocketCommand : IRocketCommand
    {
        public RegisteredRocketCommand(IRocketPluginProvider provider, string name, IRocketCommand command)
        {
            Provider = provider;
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
        public IRocketPluginProvider Provider { get; private set; }

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
