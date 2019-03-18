using System.Linq;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public class JsonNetConfigurationSection : JsonNetConfigurationElement, IConfigurationSection
    {
        public JsonNetConfigurationSection(IConfiguration root, IConfigurationElement parentElement, JToken node, string key,
                                           SectionType type) :
            base(root, parentElement, node, type)
        {
            GuardPath(key);
            Key = key;
        }

        public string Key { get; }

        public override string Path => Node.Path;

        public override IConfigurationElement Clone()
        {
            JToken node = Node.DeepClone();
            return new JsonNetConfigurationSection(null, null, node, Key, Type);
        }

        public override T Get<T>() => Node.ToObject<T>();

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