using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfigurationSection : JsonConfigurationBase, IConfigurationSection
    {
        private readonly string key;

        public JsonConfigurationSection(JToken token, string key) : base(token)
        {
            this.key = key;
        }

        public string Key => key;

        public string Path => Node.Path;
    }
}