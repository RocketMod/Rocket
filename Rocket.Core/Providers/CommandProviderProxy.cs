using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API.Commands;
using Rocket.API.Providers;

namespace Rocket.Core.Providers.Commands
{
    public class CommandProviderProxy : ProxyBase<ICommandProvider>, ICommandProvider
    {
        public ReadOnlyCollection<Type> Commands
        {
            get
            {
                List<Type> result = new List<Type>();
                foreach (ICommandProvider provider in R.Providers.GetProviders<ICommandProvider>())
                {
                    result.AddRange(provider.Commands);
                }
                return result.AsReadOnly();
            }
        }
    }
}