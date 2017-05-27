using Rocket.API.Commands;
using Rocket.API.Providers.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rocket.Core.Commands
{
    public class RocketBuiltinCommandProvider : IRocketCommandProvider
    {
        public void Unload(bool isReload = false)
        {

        }

        public void Load(bool isReload = false)
        {
            //do nothing
        }

        public ReadOnlyCollection<IRocketCommand> Commands { get; } = new List<IRocketCommand>
        {
            new CommandExit(),
            //new CommandHelp(),
            //new CommandP(),
            new CommandRocket()
        }.AsReadOnly();
    }
}
