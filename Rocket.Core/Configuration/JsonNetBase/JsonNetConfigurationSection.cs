using System.Linq;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public class JsonNetConfigurationSection : JsonNetConfigurationElement, IConfigurationSection
    {
        public JsonNetConfigurationSection(IConfiguration root, IConfigurationElement parent, JToken node, string key) :
            base(root, parent, node)
        {
            Key = key;
        }

        public string Key { get; }

        public override string Path => Node.Path;

        public bool HasValue
        {
            get
            {
                JToken node = Node;

                if (node is JProperty p)
                    return p.Value.Type == JTokenType.Null;

                if (node is JArray a)
                    return !a.Children().Any();

                return !node.HasValues;
            }
        }
    }
}