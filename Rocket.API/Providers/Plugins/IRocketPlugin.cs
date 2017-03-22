using Rocket.API.Assets;
using Rocket.API.Collections;

namespace Rocket.API.Providers.Plugins
{
    public enum PluginState { Loaded, Unloaded, Failure, Cancelled };

    public interface IRocketPlugin<TConfiguration> : IRocketPlugin where TConfiguration : class
    {
        IAsset<TConfiguration> Configuration { get; }
    }

    public interface IRocketPlugin
    {
        string Name { get; }
        PluginState State { get; }
        TranslationList DefaultTranslations { get; }
        string WorkingDirectory { get; }

        void LoadPlugin();
        void UnloadPlugin(PluginState state = PluginState.Unloaded);
        void ReloadPlugin();
        void DestroyPlugin();
    }
}