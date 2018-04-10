using System.Runtime.Remoting.Channels;
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

        public string Path => Node.Path;
    }
}