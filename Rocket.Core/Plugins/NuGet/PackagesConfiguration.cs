using Rocket.Core.Configuration;

namespace Rocket.Core.Plugins.NuGet
{
    public class PackagesConfiguration
    {
        [ConfigArray("NuGetPackage")]
        public ConfigurationNuGetPackage[] NugetPackages { get; set; } = new ConfigurationNuGetPackage[0];
    }

    public class ConfigurationNuGetPackage
    {
        public string Id { get; set; }
        public string Version { get; set; }
    }
}