using System.Collections.Generic;
using RestSharp.Deserializers;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetRepository
    {
        public string Version { get; set; }

        public List<NuGetResource> Resources { get; set; }

        [DeserializeAs(Attribute = false, Name = "@context")]
        public NuGetContext Context { get; set; }

        public string BaseUrl { get; set; }
    }
}