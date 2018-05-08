using System.Collections.Generic;
using Rocket.API.Configuration;

namespace Rocket.API.I18N
{
    /// <summary>
    ///     Provides translations.
    /// </summary>
    public interface ITranslationCollection
    {
        /// <summary>
        ///     Get a localized message from the given translation key.
        /// </summary>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments. See <see cref="string.Format(string, object[])"/>.</param>
        /// <returns>the translated message</returns>
        string Get(string translationKey, params object[] arguments);

        /// <summary>
        ///     Sets the format for a translation key.
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="value"></param>
        void Set(string translationKey, string value);

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
        ///     Saves the changes of <see cref="Set"/>.
        /// </summary>
        void Save();
    }
}