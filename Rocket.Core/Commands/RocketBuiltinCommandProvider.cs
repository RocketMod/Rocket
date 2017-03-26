using Rocket.API.Commands;
using Rocket.API.Providers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Providers.Commands;
using System.Collections.ObjectModel;

namespace Rocket.Core.Commands
{
    public class RocketBuiltinCommandProvider : IRocketCommandProvider
    {
        public override void Unload()
        {

        }

        public override void Load(bool isReload = false)
        {
            //do nothing
        }

        public ReadOnlyCollection<IRocketCommand> Commands { get; } = new List<IRocketCommand>
        {
            new CommandExit(),
            new CommandHelp(),
            new CommandP(),
            new CommandRocket()
        }.AsReadOnly());
    }
}
