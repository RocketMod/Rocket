using Rocket.API;

namespace Rocket.Core
{
    public class ConfigurationContext : IConfigurationContext
    {
        public string WorkingDirectory { get; set; }
        public string ConfigurationName { get; set; }
    }
}