using System.Linq;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public class JsonNetConfigurationSection : JsonNetConfigurationElement, IConfigurationSection
    {
        private readonly string key;

        public JsonNetConfigurationSection(IConfiguration root, IConfigurationElement parent, JToken node, string key) : base(root, parent, node)
        {
            this.key = key;
        }

        public string Key => key;

        public override string Path => Node.Path;

        public bool HasValue
        {
            get
            {
                var node = Node;

                if (node is JProperty p)
                    return p.Value.Type == JTokenType.Null;

                if (node is JArray a)
                    return !a.Children().Any();

                return !node.HasValues;
            }
        }
    }
}