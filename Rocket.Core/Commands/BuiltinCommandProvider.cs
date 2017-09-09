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
        public ReadOnlyCollection<Type> Commands { get; } = new List<Type>
        {
            typeof(CommandExit),
            //new CommandHelp(),
            //new CommandP(),
            typeof(CommandRocket)
        }.AsReadOnly();
    }
}
