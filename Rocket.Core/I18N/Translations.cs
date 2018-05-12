using System;
using System.Collections.Generic;
using Rocket.API.Configuration;
using Rocket.API.I18N;

namespace Rocket.Core.I18N
{
    public class TranslationCollection : ITranslationCollection, IFormattable
    {
        private readonly IConfiguration config;

        public TranslationCollection(IConfiguration config)
        {
            this.config = config;
        }

        public string ToString(string format, IFormatProvider formatProvider) => Get(format);

        public string Get(string translationKey, params object[] arguments)
            => string.Format(config[translationKey].Get<string>(), arguments);

        public void Set(string translationKey, string format)
        {
            if (!config.ChildExists(translationKey))
                config.CreateSection(translationKey, SectionType.Value);
            config[translationKey].Set(format);
        }

        public void Load(IConfigurationContext context, Dictionary<string, string> defaultConfiguration)
        {
            if (config.IsLoaded)
                throw new Exception("Translation locator was already loaded");

            config.ConfigurationContext = context;
            config.Load(new { });
            foreach (KeyValuePair<string, string> pair in defaultConfiguration)
            {
                if (config.ChildExists(pair.Key))
                    continue;

                config.CreateSection(pair.Key, SectionType.Value);
                config[pair.Key].Set(pair.Value);
            }

            config.Save();
        }

        public void Reload()
        {
            config.Reload();
        }

        public void Save()
        {
            config.Save();
        }

        public string ServiceName => "RocketTranslations";
    }
}