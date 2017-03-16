using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Serialisation;

namespace Rocket.API.Plugins
{
    public enum PluginState { Loaded, Unloaded, Failure, Cancelled };

    public interface IRocketPlugin<TConfiguration> : IRocketPlugin where TConfiguration : class
    {
        IAsset<TConfiguration> Configuration { get; }
    }

    public delegate void RocketPluginUnloading(RocketPluginBase plugin);
    public delegate void RocketPluginUnloaded(RocketPluginBase plugin);
    public delegate void RocketPluginLoading(RocketPluginBase plugin, ref bool cancelLoading);
    public delegate void RocketPluginLoaded(RocketPluginBase plugin);

    public interface IRocketPlugin
    {
        string Name { get; }
        PluginState State { get; }
        TranslationList DefaultTranslations { get; }
        IAsset<TranslationList> Translations { get; }
        string WorkingDirectory { get; }

        void LoadPlugin();
        void UnloadPlugin(PluginState state = PluginState.Unloaded);
        void ReloadPlugin();

        event RocketPluginUnloading OnPluginUnloading;
        event RocketPluginUnloaded OnPluginUnloaded;

        event RocketPluginLoading OnPluginLoading;
        event RocketPluginLoaded OnPluginLoaded;
    }
}