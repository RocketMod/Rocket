using RestSharp.Deserializers;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetContext
    {
        [DeserializeAs(Attribute = true, Name = "@vocab")]
        public string Vocab { get; set; }

        public string Comment { get; set; }
    }
}