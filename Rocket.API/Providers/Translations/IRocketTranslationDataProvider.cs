using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.API.Providers.Plugins;

namespace Rocket.API.Providers.Translations
{
    public interface IRocketTranslationDataProvider : IRocketDataProviderBase
    {
        string Translate(string key, string language, params object[] args);
        string Translate(IRocketPlugin plugin,string key, string language, params object[] args);

        void RegisterDefaultTranslations(IRocketPlugin plugin, TranslationList defaultTranslations);
        void RegisterDefaultTranslations(TranslationList defaultTranslations);
    }
}