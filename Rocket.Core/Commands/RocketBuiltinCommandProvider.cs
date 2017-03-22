using Rocket.API.Commands;
using Rocket.API.Providers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rocket.Core.Commands
{
    public class RocketBuiltinCommandProvider : IRocketCommandProvider
    {
        public ReadOnlyCollection<IRocketCommand> Commands
        {
            get
            {
                return new List<IRocketCommand>
                {
                    new CommandExit(),
                    new CommandHelp(),
                    new CommandP(),
                    new CommandRocket()
                }.AsReadOnly();
            }
        }

        public void Load()
        {

        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {

        }
    }
}
