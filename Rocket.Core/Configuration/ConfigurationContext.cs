using Rocket.API.Configuration;

namespace Rocket.Core.Configuration
{
    public class ConfigurationContext : IConfigurationContext
    {
        public ConfigurationContext()
        {

        }

        public ConfigurationContext(string workingDirectory, string configurationName)
        {
            WorkingDirectory = workingDirectory;
            ConfigurationName = configurationName;
        }

        public ConfigurationContext(IConfigurationContext context) : this(context.WorkingDirectory, context.ConfigurationName)
        {
        }

        public string WorkingDirectory { get; set; }
        public string ConfigurationName { get; set; }
    }
}