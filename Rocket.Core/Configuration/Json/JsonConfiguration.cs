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
        public JsonConfiguration() : base(null)
        {
            Root = this;
        }

        private string file;

        public bool Exist(IEnvironmentContext context)
        {
            return File.Exists(System.IO.Path.Combine(context.WorkingDirectory, context.Name + ".json"));
        }

        public void Load(IEnvironmentContext context, object defaultConfiguration)
        {
            file = System.IO.Path.Combine(context.WorkingDirectory, context.Name + ".json");

            if (!File.Exists(file))
            {
                LoadFromObject(defaultConfiguration);
                Save();
                return;
            }

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
            IsLoaded = true;
        }

        public void LoadFromObject(object o)
        {
            Node = JObject.FromObject(o);
            IsLoaded = true;
        }

        public void LoadEmpty()
        {
            Node = new JObject();
            IsLoaded = true;
        }

        public void Reload()
        {
            GuardLoaded();
            if (file == null)
                return;

            LoadFromJson(File.ReadAllText(file));
        }

        public void Save()
        {
            GuardLoaded();

            if (file == null)
                throw new NotSupportedException(
                    "This configuration was not loaded from a file; so it can not be saved!");

            File.WriteAllText(file, Node.ToString(Formatting.Indented));
        }

        public bool IsLoaded { get; protected set; }

        public override string Path => "";

        public override void Set(object o)
        {
            GuardLoaded();
            Node = JObject.FromObject(o);
        }
    }
}