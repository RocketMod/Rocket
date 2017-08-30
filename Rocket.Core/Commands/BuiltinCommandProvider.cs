using Rocket.API.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.Core.Providers;
using System;
using Rocket.API.Providers;

namespace Rocket.Core.Commands
{
   
    public class BuiltinCommandProvider : ProviderBase, ICommandProvider
    {
        protected override void OnLoad(ProviderManager providerManager)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<Type> Commands { get; } = new List<Type>
        {
            typeof(CommandExit),
            //new CommandHelp(),
            //new CommandP(),
            typeof(CommandRocket)
        }.AsReadOnly();
    }
}
