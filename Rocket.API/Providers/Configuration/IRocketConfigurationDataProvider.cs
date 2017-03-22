using Rocket.API.Plugins;

namespace Rocket.API.Providers.Configuration
{
    public interface IRocketConfigurationDataProvider : IRocketDataProviderBase
    {
        TConfiguration GetPluginConfiguration<TConfiguration>(IRocketPlugin plugin) where TConfiguration : class;
        TConfiguration GetProviderConfiguration<TConfiguration>(RocketProviderBase plugin) where TConfiguration : class;
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class;
    }
}