using RestSharp.Deserializers;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetResource
    {
        [DeserializeAs(Attribute = true, Name = "@id")]
        public string Id { get; set; }

        [DeserializeAs(Attribute = true, Name = "@type")]
        public string Type { get; set; }

        public string Comment { get; set; }
    }
}