using Rocket.API.Plugins;

namespace Rocket.API.Providers
    public interface IRocketConfigurationDataProvider
    {
        TConfiguration GetPluginConfiguration<TConfiguration>(IRocketPlugin plugin) where TConfiguration : class;
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class;
    }
}