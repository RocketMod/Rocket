using System.Collections.Generic;

namespace Rocket.API.I18N
{
    public interface ITranslatable
    {
        ITranslationLocator Translations { get; }

        Dictionary<string, string> DefaultTranslations { get; }
    }
}