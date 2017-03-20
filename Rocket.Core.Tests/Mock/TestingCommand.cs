using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Core.Tests.Mock
{
    public class TestingCommand : IRocketCommand
    {
        public TestingCommand(string name)
        {
            Name = name;
        }

        private List<string> aliases = new List<string>();

        private AllowedCaller allowedCaller = AllowedCaller.Both;

        private string help = "Test Help";

        private string name ="test";

        private List<string> permissions = new List<string>(){"test"};

        private string syntax = "";

        public List<string> Aliases
        {
            get
            {
                return aliases;
            }

            set
            {
                aliases = value;
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return allowedCaller;
            }

            set
            {
                allowedCaller = value;
            }
        }

        public string Help
        {
            get
            {
                return help;
            }

            set
            {
                help = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public List<string> Permissions
        {
            get
            {
                return permissions;
            }

            set
            {
                permissions = value;
            }
        }

        public string Syntax
        {
            get
            {
                return syntax;
            }

            set
            {
                syntax = value;
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
