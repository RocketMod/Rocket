using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API.Commands;
using Rocket.API.Providers;
using Rocket.API.Providers.Commands;

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