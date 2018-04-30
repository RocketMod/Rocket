using System.Collections.Generic;

namespace Rocket.API.I18N
{
    /// <summary>
    ///     An object which has translations.
    /// </summary>
    public interface ITranslatable
    {
        /// <summary>
        ///     The translation source.
        /// </summary>
        ITranslationLocator Translations { get; }

        /// <summary>
        ///     The default translations.
        /// </summary>
        Dictionary<string, string> DefaultTranslations { get; }
    }
}