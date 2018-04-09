using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.API;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfiguration : JsonConfigurationBase, IConfiguration
    {
        private bool isLoaded;
        private string file;
        public JsonConfiguration() : base(null) { }

        public void Load(IEnvironmentContext context)
        {
            file = Path.Combine(context.WorkingDirectory, context.Name + ".json");
            if(!File.Exists(file))
                File.WriteAllText(file, "");

            string json = File.ReadAllText(file);
            LoadFromJson(json);
        }

        public void LoadFromJson(string json)
        {
            Node = JObject.Parse(json, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore
            });
            isLoaded = true;
        }

        public void Reload()
        {
            if (file == null)
                return;

            LoadFromJson(File.ReadAllText(file));
        }

        public void Save()
        {
            if(file == null)
                throw new NotSupportedException("This configuration was not loaded from a file; so it can not be saved!");

            File.WriteAllText(file, Node.ToString(Formatting.Indented));
        }

        public bool IsLoaded => isLoaded;
    }
}