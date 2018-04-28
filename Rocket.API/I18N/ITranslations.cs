using System.Collections.Generic;

namespace Rocket.API.I18N
{
    public interface ITranslationLocator
    {
        /// <summary>
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="bindings"></param>
        /// <returns>The localized message</returns>
        string GetLocalizedMessage(string translationKey, params object[] bindings);

        /// <summary>
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="message"></param>
        /// ß
        void SetLocalizedMessage(string translationKey, string message);

        void Load(IConfigurationContext context, Dictionary<string, string> defaultConfiguration);

        void Reload();

        void Save();
    }
}