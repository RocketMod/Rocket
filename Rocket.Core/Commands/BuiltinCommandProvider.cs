using Rocket.API.Commands;
using Rocket.API.Providers.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.Core.Providers;
using System;

namespace Rocket.Core.Commands
{
   
    public class BuiltinCommandProvider : ProviderBase, IRocketCommandProvider
    {
        protected override void OnLoad(ProviderManager providerManager)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
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
