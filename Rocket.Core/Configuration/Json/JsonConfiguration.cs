using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfiguration : IConfiguration
    {
        public JToken Node { get; protected set; }

        public JsonConfiguration(JToken node)
        {
            Node = node;
        }

        public string this[string key]
        {
            get
            {
                GuardLoaded();
                return Node[key].Value<string>();
            }
            set
            {
                GuardLoaded();
                Node[key] = value;
            }
        }

        public IConfigurationSection GetSection(string key)
        {
            GuardLoaded();
            return new JsonConfigurationSection(Node[key]);
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            GuardLoaded();

            List<IConfigurationSection> sections = new List<IConfigurationSection>();
            foreach (JToken node in Node.Children())
            {
                sections.Add(new JsonConfigurationSection(node));
            }

            return sections;
        }

        public void GuardLoaded()
        {
            if(Node == null)
                throw new ConfigurationNotLoadedException();
        }
    }
}