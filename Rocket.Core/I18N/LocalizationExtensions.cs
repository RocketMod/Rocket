using System.Drawing;
using Rocket.API.I18N;
using Rocket.API.User;
using Rocket.Core.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rocket.Core.I18N
{
    public static class LocalizationExtensions
    {
        /// <summary>
        ///     Sends a localized (translatable) message to the user.
        /// </summary>
        /// <param name="user">The message receiver.</param>
        /// <param name="translations">The translations source.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="color">The color to use.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" /></param>
        public static async Task SendLocalizedMessageAsync(this IUser user, ITranslationCollection translations,
                                                string translationKey, Color? color = null, params object[] arguments)
        {
            await user.SendMessageAsync(await translations.GetAsync(translationKey, arguments), color);
        }

        /// <summary>
        ///     Sends a localized (translatable) message to the user.
        /// </summary>
        /// <param name="user">The message receiver.</param>
        /// <param name="translations">The translations source.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" /></param>
        public static async Task SendLocalizedMessageAsync(this IUser user, ITranslationCollection translations,
            string translationKey, params object[] arguments)
        {
            await user.SendMessageAsync(await translations.GetAsync(translationKey, arguments));
        }

        /// <summary>
        ///     Sends a localized message to the given player
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation source.</param>
        /// <param name="user">The message receiver.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="color">The color to use.</param>
        /// <param name="arguments">The arguments for the message.</param>
        public static async Task SendLocalizedMessageAsync(this IUserManager userManager, ITranslationCollection translations,
                                                IUser user, string translationKey, Color? color = null, params object[] arguments)
        {
            await userManager.SendMessageAsync(user, await translations.GetAsync(translationKey, arguments), color);
        }

        /// <summary>
        ///     Sends a localized message to the given player
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation source.</param>
        /// <param name="user">The message receiver.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message.</param>
        public static async Task SendLocalizedMessageAsync(this IUserManager userManager, ITranslationCollection translations,
            IUser user, string translationKey, params object[] arguments)
        {
            await userManager.SendMessageAsync(user, await translations.GetAsync(translationKey, arguments));
        }


        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="color">The color to use.</param>
        /// <param name="arguments">The arguments for the message</param>
        public static async Task BroadcastLocalizedAsync(this IUserManager userManager, ITranslationCollection translations,
                                              string translationKey, Color? color = null, params object[] arguments)
        {
            await userManager.BroadcastAsync(null, await translations.GetAsync(translationKey, arguments), color);
        }

        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="arguments">The arguments for the message</param>
        public static async Task BroadcastLocalizedAsync(this IUserManager userManager, ITranslationCollection translations,
            string translationKey, params object[] arguments)
        {
            await userManager.BroadcastAsync(null, await translations.GetAsync(translationKey, arguments));
        }

        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="color">The color to use.</param>
        /// <param name="arguments">The arguments for the message</param>
        public static async Task BroadcastLocalizedAsync(this IUserManager userManager, ITranslationCollection translations,
            IEnumerable<IUser> receivers, string translationKey, Color? color = null, params object[] arguments)
        {
            await userManager.BroadcastAsync(null, receivers, await translations.GetAsync(translationKey, arguments), color);
        }

        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="arguments">The arguments for the message</param>
        public static async Task BroadcastLocalizedAsync(this IUserManager userManager, ITranslationCollection translations,
            IEnumerable<IUser> receivers, string translationKey, params object[] arguments)
        {
            await userManager.BroadcastAsync(null, receivers, await translations.GetAsync(translationKey, arguments));
        }
    }
}