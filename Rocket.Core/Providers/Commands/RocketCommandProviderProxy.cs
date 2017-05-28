using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Commands;
using Rocket.Core.Player;

namespace Rocket.Core.Providers.Commands
{
    [RocketProviderProxy]
    public class RocketCommandProviderProxy : IRocketCommandProvider
    {
        public ReadOnlyCollection<IRocketCommand> Commands
        {
            get
            {
                List<IRocketCommand> result = new List<IRocketCommand>();
                foreach (IRocketCommandProvider provider in R.Providers.GetProviders<IRocketCommandProvider>())
                {
                    result.AddRange(provider.Commands);
                }
                return result.AsReadOnly();
            }
        }

        public void Load(bool isReload = false)
        {

        }

        public void Unload(bool isReload = false)
        {

        }
    }
}