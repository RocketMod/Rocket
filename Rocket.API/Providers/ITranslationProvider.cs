using Rocket.API.Collections;
using Rocket.API.Plugins;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface ITranslationProvider
    {
        string Translate(string key, params object[] args);
        string Translate(IPlugin plugin,string key, params object[] args);

        string TranslateLanguage(string key, string language, params object[] args);
        string TranslateLanguage(IPlugin plugin, string key, string language, params object[] args);

        void RegisterDefaultTranslations(IPlugin plugin, TranslationList defaultTranslations);
        void RegisterDefaultTranslations(TranslationList defaultTranslations);
        string GetCurrentLanguage();
    }
}