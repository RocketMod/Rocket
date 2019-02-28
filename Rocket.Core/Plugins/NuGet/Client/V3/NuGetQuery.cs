namespace Rocket.Core.Plugins.NuGet.Client.V3 {
    public class NuGetQuery
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public bool PreRelease { get; set; }
    }
}