using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration
{
    public class RocketConfigurationProvider : IRocketConfigurationProvider
    {
        private readonly IRuntime runtime;
        private readonly IConfiguration configuration;

        public RocketConfigurationProvider(IRuntime runtime, IConfiguration configuration)
        {
            this.runtime = runtime;
            this.configuration = configuration;
        }

        public async Task LoadAsync()
        {
            ConfigurationContext context = new ConfigurationContext(runtime, "Configuration");
            await configuration.LoadAsync(context, Configuration);
            Configuration = configuration.Get(Configuration);
        }

        public async Task ReloadAsync()
        {
            await configuration.ReloadAsync();
        }

        public async Task SaveAsync()
        {
            configuration.Set(Configuration);
            await configuration.SaveAsync();
        }

        public RocketConfiguration Configuration { get; private set; } = new RocketConfiguration();
    }
}