using System;
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

        public string GetLocalizedMessage(string translationKey, params object[] bindings)
        {
            return string.Format(config[translationKey], bindings);
        }

        public void SetLocalizedMessage(string translationKey, string message)
        {
            config[translationKey] = message;
        }

        public void Load(IEnvironmentContext context)
        {
            if (config.IsLoaded)
                throw new Exception("Permission provider is already loaded");

            config.Load(context);
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