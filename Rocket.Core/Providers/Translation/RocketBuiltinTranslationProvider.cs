using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.API.Providers;
using Rocket.API.Providers.Plugins;
using Rocket.API.Providers.Translations;

namespace Rocket.Core.Providers.Translation
{
    [RocketProviderImplementation]
    public class RocketBuiltinTranslationProvider : IRocketTranslationDataProvider
    {
        //TODO: Implement file loading
        private readonly Dictionary<IRocketPlugin, List<TranslationList>> _defaultPluginTranslations = new Dictionary<IRocketPlugin, List<TranslationList>>();
        private readonly List<TranslationList> _defaultTranslations = new List<TranslationList>();
        public void Unload(bool isReload = false)
        {
            //
        }

        public void Load(bool isReload = false)
        {
            //
        }

        public void Save()
        {
            // 
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
            if (!_defaultPluginTranslations.ContainsKey(plugin))
                _defaultPluginTranslations.Add(plugin, new List<TranslationList>());

            _defaultPluginTranslations[plugin].Add(defaultTranslations);
        }

        public void RegisterDefaultTranslations(TranslationList defaultTranslations)
        {
            _defaultTranslations.Add(defaultTranslations);
        }

        public string GetCurrentLanguage()
        {
            return "en";
        }

        public string TranslateLanguage(string key, string language, params object[] args)
        {
            foreach (var translationList in _defaultTranslations)
            {
                var t = translationList.Translate(key);
                if (t != null)
                    return t;
            }

            return null;
        }

        public string TranslateLanguage(IRocketPlugin plugin, string key, string language, params object[] args)
        {
            if (!_defaultPluginTranslations.ContainsKey(plugin))
                return null;

            foreach (var translationList in _defaultPluginTranslations[plugin])
            {
                var t = translationList.Translate(key);
                if (t != null)
                    return t;
            }

            return null;
        }
    }
}
