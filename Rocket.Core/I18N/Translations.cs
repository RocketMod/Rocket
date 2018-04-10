using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.I18N;

namespace Rocket.Core.I18N
{
    public class Translations : ITranslations
    {
        private readonly IConfiguration config;

        public Translations(IConfiguration config)
        {
            this.config = config;
        }

        public string GetLocalizedMessage(string translationKey, params object[] bindings) => string.Format(config[translationKey].Get<string>(), bindings);

        public void SetLocalizedMessage(string translationKey, string message)
        {
            config[translationKey].Set(message);
        }

        public void Load(IEnvironmentContext context, Dictionary<string, string> defaultConfiguration)
        {
            if (config.IsLoaded)
                throw new Exception("Permission provider is already loaded");

            bool isNew = config.Exist(context);
            config.Load(context, new {});

            if (isNew)
            {
                foreach (var pair in defaultConfiguration)
                {
                    config.CreateSection(pair.Key, SectionType.Value);
                    config[pair.Key].Set(pair.Value);
                }
            }
        }

        public void Reload()
        {
            config.Reload();
        }

        public void Save()
        {
            config.Save();
        }
    }
}