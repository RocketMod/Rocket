using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.JsonNetBase;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfiguration : JsonNetConfigurationBase
    {
        protected override string FileEnding => "json";
        public override string Name => "Json";

        public void LoadFromJson(string json)
        {
            JObject tmp = JObject.Parse(json, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore
            });

            if (Node == null)
                LoadEmpty();

            DeepCopy(tmp, (JObject) Node);
            IsLoaded = true;
        }

        public override void LoadFromFile(string file)
        {
            string json = File.ReadAllText(file);
            LoadFromJson(json);
        }

        public override void SaveToFile(string file)
        {
            string json = ToJson();
            File.WriteAllText(file, json);
        }

        public override IConfigurationElement Clone()
        {
            JsonConfiguration config = new JsonConfiguration();
            config.LoadFromJson(ToJson());
            return config;
        }

        public string ToJson() => Node.ToString(Formatting.Indented);
    }
}