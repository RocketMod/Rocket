using Rocket.API;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration
{
    public class RocketSettingsProvider : IRocketSettingsProvider
    {
        public RocketSettingsProvider(IRuntime runtime, IConfiguration configuration)
        {
            ConfigurationContext context = new ConfigurationContext(runtime, "Configuration");
            RocketSettings defaultVal = new RocketSettings();
            configuration.Load(context, defaultVal);
            Settings = configuration.Get(defaultVal);
        }

        public RocketSettings Settings { get; }
    }
}