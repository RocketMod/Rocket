using Rocket.API.Providers.Plugins;

namespace Rocket.API.Providers.Configuration
{
    [RocketProvider]
    public interface IRocketConfigurationDataProvider : IRocketDataProviderBase
    {
        TConfiguration GetPluginConfiguration<TConfiguration>(IRocketPlugin plugin) where TConfiguration : class;
        TConfiguration GetProviderConfiguration<TConfiguration>(IRocketProviderBase provider) where TConfiguration : class;
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class;
    }
}