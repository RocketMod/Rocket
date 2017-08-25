using System.Collections.ObjectModel;

namespace Rocket.API.Providers.Plugins
{
    [ProviderDefinition]
    public interface IRocketPluginProvider
    {
        ReadOnlyCollection<IRocketPlugin> Plugins { get; }
        IRocketPlugin GetPlugin(string name);
        void LoadPlugins();
    }
}
