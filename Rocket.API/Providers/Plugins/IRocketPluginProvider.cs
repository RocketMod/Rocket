using System;
using System.Collections.ObjectModel;

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
