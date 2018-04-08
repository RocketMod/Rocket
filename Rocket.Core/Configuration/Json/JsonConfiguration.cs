using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;
#if NET35
using Rocket.Core.Extensions; //backport Stream.CopyTo(...)
#endif

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfiguration : JsonConfigurationBase, IConfiguration
    {
        public JsonConfiguration() : base(null) { }

        public void Load(Stream stream)
        {
            string json = stream.ConvertToString(Encoding.UTF8);
            Node = JObject.Parse(json, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore
            });
        }

        public void Save(Stream stream)
        {
            stream.Write(Node.ToString(Formatting.Indented));
        }
    }
}