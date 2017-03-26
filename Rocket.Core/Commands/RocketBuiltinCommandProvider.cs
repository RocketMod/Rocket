using Rocket.API.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Providers.Commands;

namespace Rocket.Core.Commands
{
    public class RocketBuiltinCommandProvider : RocketProviderBase, IRocketCommandProvider
    {
        public override void Unload()
        {

        }

        public override void Load(bool isReload = false)
        {
            //do nothing
        }

        public List<IRocketCommand> Commands { get; } = new List<IRocketCommand>
        {
            new CommandExit(),
            new CommandHelp(),
            new CommandP(),
            new CommandRocket()
        };
    }
}
