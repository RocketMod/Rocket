using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Rocket.API;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public abstract class JsonNetConfigurationBase : JsonNetConfigurationElement, IConfiguration
    {
        protected JsonNetConfigurationBase() : base(null)
        {
            Root = this;
        }

        private string file;

        public bool Exist(IConfigurationContext context)
        {
            return File.Exists(System.IO.Path.Combine(context.WorkingDirectory, context.ConfigurationName + ".json"));
        }

        public void SetContext(IConfigurationContext context)
        {
            file = System.IO.Path.Combine(context.WorkingDirectory, context.ConfigurationName + ".json");
        }

        public virtual void Load(IConfigurationContext context, object defaultConfiguration)
        {
            SetContext(context);

            if (!File.Exists(file))
            {
                LoadFromObject(defaultConfiguration);
                Save();
                return;
            }

            LoadFromFile(file);
        }

        protected abstract void LoadFromFile(string file);

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

            LoadFromFile(file);
        }

        public void Save()
        {
            GuardLoaded();

            if (file == null)
                throw new NotSupportedException(
                    "This configuration was not loaded from a file; so it can not be saved!");

            SaveToFile(file);
        }

        protected abstract void SaveToFile(string file);

        public bool IsLoaded { get; protected set; }

        public override string Path => "";

        public override void Set(object o)
        {
            GuardLoaded();
            Node = JObject.FromObject(o);
        }
    }
}