using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.Core.Tests.Mock
{
    public class TestingCommand : IRocketCommand
    {
        public List<string> Aliases { get; set; } = new List<string>();

        public AllowedCaller AllowedCaller { get; set; } = AllowedCaller.Both;

        public string Help { get; set; } = "Test Help";

        public string Name { get; set; } = "test";

        public List<string> Permissions { get; set; } = new List<string> {"test"};

        public string Syntax { get; set; } = "";

        public void Execute(ICommandContext ctx)
        {
            ctx.Print("yay");
        }
    }
}
