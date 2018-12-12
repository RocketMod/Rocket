using System.Threading.Tasks;
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

        public async Task LoadAsync()
        {
            ConfigurationContext context = new ConfigurationContext(runtime, "Configuration");
            await configuration.LoadAsync(context, Settings);
            Settings = configuration.Get(Settings);
        }

        public async Task ReloadAsync()
        {
            await configuration.ReloadAsync();
        }

        public async Task SaveAsync()
        {
            configuration.Set(Settings);
            await configuration.SaveAsync();
        }

        public RocketSettings Settings { get; private set; } = new RocketSettings();
    }
}