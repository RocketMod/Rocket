using Rocket.API.Configuration;

namespace Rocket.Core.Configuration
{
    public class ConfigurationContext : IConfigurationContext
    {
        public ConfigurationContext() { }

        public ConfigurationContext(string workingDirectory, string configurationName)
        {
            WorkingDirectory = workingDirectory;
            ConfigurationName = configurationName;
        }

        public ConfigurationContext(IConfigurationContext context, string childName = null) : this(
            context.WorkingDirectory, context.ConfigurationName)
        {
            ConfigurationName += "." + childName;
        }

        public string WorkingDirectory { get; set; }
        public string ConfigurationName { get; set; }
    }
}