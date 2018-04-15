using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        protected override void LoadFromFile(string file)
        {
            string json = File.ReadAllText(file);
            LoadFromJson(json);
        }

        protected override void SaveToFile(string file)
        {
            string json = Node.ToString(Formatting.Indented);
            File.WriteAllText(file, json);
        }
    }
}