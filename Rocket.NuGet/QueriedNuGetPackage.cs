using System.Collections.Generic;
using NuGet.Protocol.Core.Types;

namespace Rocket.NuGet
{
    public class QueriedNuGetPackage
    {
        public IPackageSearchMetadata Metadata { get; set; }

        public IEnumerable<VersionInfo> Versions { get; set; }

        public string Repository { get; set; }
    }
}