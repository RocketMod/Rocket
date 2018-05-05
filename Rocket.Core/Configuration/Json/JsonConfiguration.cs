using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.JsonNetBase;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfiguration : JsonNetConfigurationBase
    {
        public void LoadFromJson(string json)
        {
            Node = JObject.Parse(json, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore
            });

            IsLoaded = true;
        }

        protected override string FileEnding => "json";
        public override string Name => "Json";

        protected override void LoadFromFile(string file)
        {
            string json = File.ReadAllText(file);
            LoadFromJson(json);
        }

        protected override void SaveToFile(string file)
        {
            string json = ToJson();
            File.WriteAllText(file, json);
        }

        public override IConfigurationElement Clone()
        {
            var config = new JsonConfiguration();
            config.LoadFromJson(ToJson());
            return config;
        }

        public string ToJson()
        {
            return Node.ToString(Formatting.Indented);
        }
    }
}