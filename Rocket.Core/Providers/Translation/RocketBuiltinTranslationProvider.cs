using System;
using Rocket.API.Collections;
using Rocket.API.Providers.Plugins;
using Rocket.API.Providers.Translations;

namespace Rocket.Core.Providers.Translation
{
    public class RocketBuiltinTranslationProvider : IRocketTranslationDataProvider
    {
        public void Unload(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public string Translate(string key, params object[] args)
        {
            return TranslateLanguage(key, GetCurrentLanguage(), args);
        }

        public string Translate(IRocketPlugin plugin, string key, params object[] args)
        {
            return TranslateLanguage(plugin, key, GetCurrentLanguage(), args);
        }

        public void RegisterDefaultTranslations(IRocketPlugin plugin, TranslationList defaultTranslations)
        {
            throw new NotImplementedException();
        }

        public void RegisterDefaultTranslations(TranslationList defaultTranslations)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentLanguage()
        {
            throw new NotImplementedException();
        }

        public string TranslateLanguage(string key, string language, params object[] args)
        {
            throw new NotImplementedException();
        }

        public string TranslateLanguage(IRocketPlugin plugin, string key, string language, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
