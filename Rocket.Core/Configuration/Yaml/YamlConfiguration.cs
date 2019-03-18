using System.Dynamic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.JsonNetBase;
using YamlDotNet.Serialization;

namespace Rocket.Core.Configuration.Yaml
{
    public class YamlConfiguration : JsonNetConfigurationBase
    {
        protected override string FileEnding { get; } = "yml";
        public override string Name { get; } = "Yaml";

        private readonly IDeserializer deserializer;
        private readonly ISerializer serializer;

        public YamlConfiguration()
        {
            deserializer = new DeserializerBuilder().Build();
            serializer = new SerializerBuilder().Build();
        }

        public void LoadFromYaml(string yaml)
        {
            var reader = new StringReader(yaml);
            var yamlObject = deserializer.Deserialize(reader);
            LoadFromObject(yamlObject);
        }

        public override void LoadFromFile(string file)
        {
            string yaml = File.ReadAllText(file);
            LoadFromYaml(yaml);
        }

        public override void SaveToFile(string file)
        {
            string yaml = ToYaml();
            File.WriteAllText(file, yaml);
        }

        public override IConfigurationElement Clone()
        {
            YamlConfiguration config = new YamlConfiguration();
            config.LoadFromYaml(ToYaml());
            return config;
        }

        public string ToYaml()
        {
            var json = Node.ToString();

            var expConverter = new ExpandoObjectConverter();
            dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);
            return serializer.Serialize((object)deserializedObject);
        }
    }
}