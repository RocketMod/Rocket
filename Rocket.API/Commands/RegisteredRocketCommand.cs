using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API.Providers;

namespace Rocket.API.Commands
{
    //public static IRocketPlayer GetRocketPlayerParameter(this string[] array, int index)
    //{
    //    if (array.Length > index)
    //    {
    //        ulong id = 0;
    //        if (ulong.TryParse(array[index], out id) && id > 76561197960265728)
    //        {
    //            return new RocketPlayerBase(id.ToString());
    //        }
    //    }
    //    return R.Implementation.Players.Players.Where(p => p.DisplayName.Contains(array[index].ToString())).FirstOrDefault();
    //}

    [Serializable]
    public class RegisteredRocketCommand : ICommand
    {
        public RegisteredRocketCommand(IPluginProvider provider, string name, ICommand command)
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
        public ICommand Command;

        [XmlIgnore]
        public IPluginProvider Provider { get; private set; }

        public void Execute(ICommandContext ctx)
        {
            Command.Execute(ctx);
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
