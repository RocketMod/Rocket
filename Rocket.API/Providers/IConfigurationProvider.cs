using Rocket.API.Plugins;
using System;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface IConfigurationProvider
    {
        TConfiguration GetPluginConfiguration<TConfiguration>(IPlugin plugin) where TConfiguration : class;
        TConfiguration GetProviderConfiguration<TConfiguration>(Type provider) where TConfiguration : class;
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class;
    }
}