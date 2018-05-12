using System.Collections.Generic;
using Rocket.API.DependencyInjection;

namespace Rocket.API.I18N
{
    /// <summary>
    ///     An object which has translations.
    /// </summary>
    public interface ITranslatable: IService
    {
        /// <summary>
        ///     The translation source.
        /// </summary>
        ITranslationCollection Translations { get; }

        /// <summary>
        ///     The default translations.
        /// </summary>
        Dictionary<string, string> DefaultTranslations { get; }
    }
}