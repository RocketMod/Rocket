using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;

namespace Rocket.API.I18N
{
    /// <summary>
    ///     Provides translations.
    /// </summary>
    public interface ITranslationCollection: IProxyableService
    {
        /// <summary>
        ///     Get a localized message from the given translation key.
        /// </summary>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments. See <see cref="string.Format(string, object[])" />.</param>
        /// <returns>the translated message</returns>
        Task<string> GetAsync(string translationKey, params object[] arguments);

        /// <summary>
        ///     Sets the format for a translation key.
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="value"></param>
        Task SetAsync(string translationKey, string value);

        /// <summary>
        ///     Loads the translations from a <see cref="IConfigurationContext">configuration context</see>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultConfiguration"></param>
        Task LoadAsync(IConfigurationContext context, Dictionary<string, string> defaultConfiguration);

        /// <summary>
        ///     Reloads the translations.
        /// </summary>
        Task ReloadAsync();

        /// <summary>
        ///     Saves the changes of <see cref="SetAsync" />.
        /// </summary>
        Task SaveAsync();
    }
}