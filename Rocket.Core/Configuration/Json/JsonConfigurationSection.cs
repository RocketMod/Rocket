using System.Linq;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfigurationSection : JsonConfigurationBase, IConfigurationSection
    {
        private readonly string key;

        public JsonConfigurationSection(JToken node, string key) : base(node)
        {
            this.key = key;
        }

        public string Key => key;

        public override string Path => Node.Path;

        public bool IsNull
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