using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.API.Plugins;

namespace Rocket.API.Providers.Translations
{
    public interface IRocketTranslationDataProvider : IRocketDataProviderBase
    {
        void Translate(string key, string language);
        void Translate(IRocketPlugin plugin,string key, string language);

        void RegisterDefaultTranslations(IRocketPlugin plugin, TranslationList defaultTranslations);
        void RegisterDefaultTranslations(TranslationList defaultTranslations);
    }
}