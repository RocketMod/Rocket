using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public string ToString(string format, IFormatProvider formatProvider) => GetAsync(format).GetAwaiter().GetResult();

        public async Task<string> GetAsync(string translationKey, params object[] arguments)
            => string.Format(config[translationKey].Get<string>(), arguments);

        public async Task SetAsync(string translationKey, string format)
        {
            if (!config.ChildExists(translationKey))
                config.CreateSection(translationKey, SectionType.Value);
            config[translationKey].Set(format);
        }

        public async Task LoadAsync(IConfigurationContext context, Dictionary<string, string> defaultConfiguration)
        {
            if (config.IsLoaded)
                throw new Exception("Translation locator was already loaded");

            config.ConfigurationContext = context;
            await config.LoadAsync(new { });
            foreach (KeyValuePair<string, string> pair in defaultConfiguration)
            {
                if (config.ChildExists(pair.Key))
                    continue;

                config.CreateSection(pair.Key, SectionType.Value);
                config[pair.Key].Set(pair.Value);
            }

            await config.SaveAsync();
        }

        public async Task ReloadAsync()
        {
            await config.ReloadAsync();
        }

        public async Task SaveAsync()
        {
            await config.SaveAsync();
        }

        public string ServiceName => "RocketTranslations";
    }
}