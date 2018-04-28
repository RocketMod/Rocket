using System.Collections.Generic;

namespace Rocket.API.I18N
{
    /// <summary>
    ///     Defines an object which has its own translations.
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