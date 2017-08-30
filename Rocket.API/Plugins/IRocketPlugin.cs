using Rocket.API.Assets;
using Rocket.API.Collections;

namespace Rocket.API.Plugins
{
    public enum PluginState { Loaded, Unloaded, Failure, Cancelled };

    public interface IPlugin<TConfiguration> : IPlugin where TConfiguration : class
    {
        IAsset<TConfiguration> Configuration { get; }
    }

    public interface IPlugin
    {
        string Name { get; }
        PluginState State { get; }
        TranslationList DefaultTranslations { get; }
        string WorkingDirectory { get; }
        bool Enabled { get; set; }

        void LoadPlugin();
        void UnloadPlugin(PluginState state = PluginState.Unloaded);
        void ReloadPlugin();
        void DestroyPlugin();
    }
}