using Rocket.API;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration
{
    public class RocketSettingsProvider : IRocketSettingsProvider
    {
        private readonly IRuntime runtime;
        private readonly IConfiguration configuration;

        public RocketSettingsProvider(IRuntime runtime, IConfiguration configuration)
        {
            this.runtime = runtime;
            this.configuration = configuration;
        }

        public void Load()
        {
            ConfigurationContext context = new ConfigurationContext(runtime, "Configuration");
            configuration.Load(context, Settings);
            Settings = configuration.Get(Settings);
        }

        public void Reload()
        {
            configuration.Reload();
        }

        public void Save()
        {
            configuration.Set(Settings);
            configuration.Save();
        }

        public RocketSettings Settings { get; private set; } = new RocketSettings();
    }
}