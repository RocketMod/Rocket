using System;
using RestSharp.Deserializers;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetPackageVersionRegistration
    {
        [DeserializeAs(Attribute = true, Name = "@id")]
        public string Id { get; set; }

        public string PackageContent { get; set; }

        public string Registration { get; set; }

        public DateTime Published { get; set; }

        public bool Listed { get; set; }
    }
}