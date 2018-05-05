using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Rocket.API;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public abstract class JsonNetConfigurationBase : JsonNetConfigurationElement, IConfiguration
    {
        public string ConfigurationFile
        {
            get
            {
                if (ConfigurationContext == null)
                    throw new ConfigurationContextNotSetException(this);
                return System.IO.Path.Combine(ConfigurationContext.WorkingDirectory, ConfigurationContext.ConfigurationName + "." + FileEnding);
            }
        }

        protected abstract string FileEnding { get; }

        protected JsonNetConfigurationBase() : base(null)
        {
            Root = this;
        }

        public bool Exists(IConfigurationContext context)
            => File.Exists(System.IO.Path.Combine(context.WorkingDirectory, context.ConfigurationName + "." + FileEnding));

        public virtual void Load(object defaultConfiguration = null)
        {
            if (ConfigurationContext == null)
                throw new Exception("ConfigurationContext is null!");

            if (!File.Exists(ConfigurationFile))
            {
                LoadFromObject(defaultConfiguration);
                Save();
                return;
            }

            LoadFromFile(ConfigurationFile);
        }

        public virtual void Load(IConfigurationContext context, object defaultConfiguration = null)
        {
            ConfigurationContext = context ?? throw new ArgumentNullException(nameof(context));

            if (!File.Exists(ConfigurationFile))
            {
                LoadFromObject(defaultConfiguration);
                Save();
                return;
            }

            LoadFromFile(ConfigurationFile);
        }

        public IConfigurationContext ConfigurationContext { get; set; }

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
            if (ConfigurationFile == null)
                return;

            LoadFromFile(ConfigurationFile);
        }

        public void Save()
        {
            GuardLoaded();

            if (ConfigurationFile == null)
                throw new NotSupportedException(
                    "This configuration was not loaded from a file; so it can not be saved!");

            SaveToFile(ConfigurationFile);
        }

        public bool IsLoaded { get; protected set; }

        public override string Path => "";

        public override void Set(object o)
        {
            GuardLoaded();
            Node = JObject.FromObject(o);
        }

        protected abstract void LoadFromFile(string file);

        protected abstract void SaveToFile(string file);
    }
}