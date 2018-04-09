using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfigurationSection : JsonConfigurationBase, IConfigurationSection
    {
        public JsonConfigurationSection(JToken token) : base(token) { }

        public string Key => (Node as JProperty)?.Name;

        public string Path => Node.Path;
    }
}