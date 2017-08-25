using Rocket.API.Providers.Plugins;
using System;

namespace Rocket.API.Providers.Configuration
{
    [ProviderDefinition]
    public interface IRocketConfigurationDataProvider
    {
        TConfiguration GetPluginConfiguration<TConfiguration>(IRocketPlugin plugin) where TConfiguration : class;
        TConfiguration GetProviderConfiguration<TConfiguration>(Type provider) where TConfiguration : class;
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class;
    }
}