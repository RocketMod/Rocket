using Rocket.API.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Commands;

namespace Rocket.Core.Commands
{
    public class RocketBuiltinCommandProvider : RocketProviderBase, IRocketCommandProvider
    {
        RocketCommandList IRocketCommandProvider.Commands
        {
            get
            {
                return new RocketCommandList()
                {
                    new CommandExit(),
                    new CommandHelp(),
                    new CommandP(),
                    new CommandRocket()
                };
            }
        }

        public override void Load()
        {

        }

        public override void Unload()
        {

        }
    }
}
