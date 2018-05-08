using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public abstract class JsonNetConfigurationBase : JsonNetConfigurationElement, IConfiguration
    {
        public Type Scheme { get; set; }

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

            if (defaultConfiguration != null)
            {
                // Load model changes 
                LoadFromObject(defaultConfiguration);
            }

            if (File.Exists(ConfigurationFile))
                LoadFromFile(ConfigurationFile);

            Save(); // save model changes
        }

        public virtual void Load(IConfigurationContext context, object defaultConfiguration = null)
        {
            ConfigurationContext = context ?? throw new ArgumentNullException(nameof(context));
            Load(defaultConfiguration);
        }

        public IConfigurationContext ConfigurationContext { get; set; }
        public abstract string Name { get; }

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

            var parentDir = System.IO.Path.GetDirectoryName(ConfigurationFile);
            if (!Directory.Exists(parentDir))
                Directory.CreateDirectory(parentDir);

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