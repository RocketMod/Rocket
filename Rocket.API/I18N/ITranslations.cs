using System.Collections.Generic;

namespace Rocket.API.I18N
{
    /// <summary>
    ///     Provides translations.
    /// </summary>
    public interface ITranslationLocator
    {
        /// <summary>
        ///     Get a localized message from the given translation key.
        /// </summary>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="bindings">The bindings. See <see cref="string.Format(string, object[])"/>.</param>
        /// <returns>the localized message</returns>
        string GetLocalizedMessage(string translationKey, params object[] bindings);

        /// <summary>
        ///     Sets the format for a translation key.
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="format"></param>
        void SetFormat(string translationKey, string format);

        /// <summary>
        ///     Loads the translations from a <see cref="IConfigurationContext">configuration context</see>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultConfiguration"></param>
        void Load(IConfigurationContext context, Dictionary<string, string> defaultConfiguration);

        /// <summary>
        ///     Reloads the translations.
        /// </summary>
        void Reload();

        /// <summary>
        ///     Saves the changes of <see cref="SetFormat"/>.
        /// </summary>
        void Save();
    }
}