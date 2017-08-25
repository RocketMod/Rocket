using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API.Commands;
using Rocket.API.Providers;
using Rocket.API.Providers.Commands;

namespace Rocket.Core.Providers.Commands
{
    [ProviderProxy]
    public class CommandProviderProxy : ProviderBase, IRocketCommandProvider
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

        protected override void OnLoad(ProviderManager providerManager)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }
    }
}