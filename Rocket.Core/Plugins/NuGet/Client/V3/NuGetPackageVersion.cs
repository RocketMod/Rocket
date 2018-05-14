using RestSharp.Deserializers;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetPackageVersion
    {
        [DeserializeAs(Attribute = true, Name = "@id")]
        public string Id { get; set; }

        public string Version { get; set; }

        public int Downloads { get; set; }
    }
}