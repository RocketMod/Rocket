using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API.Collections;
using Rocket.API.Commands;

namespace Rocket.API.Providers.Plugins
{
    [RocketProvider]
    public interface IRocketPluginProvider : IRocketProviderBase
    {
        ReadOnlyCollection<IRocketPlugin> Plugins { get; }
        IRocketPlugin GetPlugin(string name);
        ReadOnlyCollection<Type> LoadProviders();
    }
}
