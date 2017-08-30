using Rocket.API.Plugins;
using System.Collections.ObjectModel;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface IPluginProvider
    {
        ReadOnlyCollection<IPlugin> Plugins { get; }
        IPlugin GetPlugin(string name);
        void LoadPlugins();
    }
}
