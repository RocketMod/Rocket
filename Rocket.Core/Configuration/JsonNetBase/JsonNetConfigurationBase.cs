using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public abstract class JsonNetConfigurationBase : JsonNetConfigurationElement, IConfiguration
    {
        protected JsonNetConfigurationBase() : base(null)
        {
            Root = this;
        }

        public string ConfigurationFile
        {
            get
            {
                if (ConfigurationContext == null)
                    throw new ConfigurationContextNotSetException(this);
                return System.IO.Path.Combine(ConfigurationContext.WorkingDirectory,
                    ConfigurationContext.ConfigurationName + "." + FileEnding);
            }
        }

        protected abstract string FileEnding { get; }
        public Type Scheme { get; set; }

        public async Task<bool> ExistsAsync(IConfigurationContext context)
            => File.Exists(System.IO.Path.Combine(context.WorkingDirectory,
                context.ConfigurationName + "." + FileEnding));

        public virtual async Task LoadAsync(object defaultConfiguration = null)
        {
            if (ConfigurationContext == null)
                throw new Exception("ConfigurationContext is null!");

            if (defaultConfiguration != null)
                LoadFromObject(defaultConfiguration);
            else
                LoadEmpty();

            if (File.Exists(ConfigurationFile))
                LoadFromFile(ConfigurationFile);

            await SaveAsync(); // save model changes
        }

        public virtual async Task LoadAsync(IConfigurationContext context, object defaultConfiguration = null)
        {
            ConfigurationContext = context ?? throw new ArgumentNullException(nameof(context));
            await LoadAsync(defaultConfiguration);
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

        public async Task ReloadAsync()
        {
            GuardLoaded();
            if (ConfigurationFile == null)
                return;

            LoadFromFile(ConfigurationFile);
        }

        public async Task SaveAsync()
        {
            GuardLoaded();

            if (ConfigurationFile == null)
                throw new NotSupportedException(
                    "This configuration was not loaded from a file; so it can not be saved!");

            string parentDir = System.IO.Path.GetDirectoryName(ConfigurationFile);
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

        public abstract void LoadFromFile(string file);

        public abstract void SaveToFile(string file);
    }
}