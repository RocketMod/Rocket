using System.Collections.Generic;
using RestSharp.Deserializers;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetPackage
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public IEnumerable<NuGetPackageVersion> Versions { get; set; }

        public string Registration { get; set; }

        [DeserializeAs(Attribute = true, Name = "@id")]
        public string PackageId { get; set; }

        [DeserializeAs(Attribute = true, Name = "@type")]
        public string PackageType { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Authors { get; set; }

        public string IconUrl { get; set; }

        public string LicenseUrl { get; set; }

        public string ProjectUrl { get; set; }

        public string Summary { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Title { get; set; }

        public int TotalDownloads { get; set; }

        public bool Verified { get; set; }
    }
}