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
    public class JsonConfigurationRoot : JsonConfiguration, IConfigurationRoot
    {
        public JsonConfigurationRoot() : base(null)
        {
        }

        public void Reload(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            string text = Encoding.UTF8.GetString(ms.ToArray());
            Node = JObject.Parse(text);
        }

        public void Save(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(Node.ToString(Formatting.Indented));
            writer.Flush();
            stream.Position = 0;
            ms.CopyTo(stream);
        }
    }
}