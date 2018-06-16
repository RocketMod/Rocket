using Rocket.API.Drawing;
using System.Collections.Generic;
using Rocket.API.I18N;
using Rocket.API.User;
using Rocket.Core.User;

namespace Rocket.Core.I18N
{
    public static class LocalizationExtensions
    {
        /// <summary>
        ///     Sends a localized (translatable) message to the user.
        /// </summary>
        /// <param name="User">The message receiver.</param>
        /// <param name="translations">The translations source.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" /></param>
        public static void SendLocalizedMessage(this IUser User, ITranslationCollection translations,
                                                string translationKey, Color? color = null, params object[] arguments)
        {
            User.SendMessage(translations.Get(translationKey, arguments), color);
        }

        /// <summary>
        ///     Sends a localized (translatable) message to the user.
        /// </summary>
        /// <param name="User">The message receiver.</param>
        /// <param name="translations">The translations source.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" /></param>
        public static void SendLocalizedMessage(this IUser User, ITranslationCollection translations,
            string translationKey, params object[] arguments)
        {
            User.SendMessage(translations.Get(translationKey, arguments));
        }

        /// <summary>
        ///     Sends a localized message to the given player
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation source.</param>
        /// <param name="user">The message receiver.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message.</param>
        public static void SendLocalizedMessage(this IUserManager userManager, ITranslationCollection translations,
                                                IUser user, string translationKey, Color? color = null, params object[] arguments)
        {
            userManager.SendMessage(user, translations.Get(translationKey, arguments), color);
        }

        /// <summary>
        ///     Sends a localized message to the given player
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation source.</param>
        /// <param name="user">The message receiver.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message.</param>
        public static void SendLocalizedMessage(this IUserManager userManager, ITranslationCollection translations,
            IUser user, string translationKey, params object[] arguments)
        {
            userManager.SendMessage(user, translations.Get(translationKey, arguments));
        }


        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="arguments">The arguments for the message</param>
        public static void BroadcastLocalized(this IUserManager userManager, ITranslationCollection translations,
                                              string translationKey, Color? color = null, params object[] arguments)
        {
            userManager.Broadcast(null, translations.Get(translationKey, arguments), color);
        }

        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="arguments">The arguments for the message</param>
        public static void BroadcastLocalized(this IUserManager userManager, ITranslationCollection translations,
            string translationKey, params object[] arguments)
        {
            userManager.Broadcast(null, translations.Get(translationKey, arguments));
        }

        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="arguments">The arguments for the message</param>
        public static void BroadcastLocalized(this IUserManager userManager, ITranslationCollection translations,
            IEnumerable<IUser> receivers, string translationKey, Color? color = null, params object[] arguments)
        {
            userManager.Broadcast(null, receivers, translations.Get(translationKey, arguments), color);
        }

        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="arguments">The arguments for the message</param>
        public static void BroadcastLocalized(this IUserManager userManager, ITranslationCollection translations,
            IEnumerable<IUser> receivers, string translationKey, params object[] arguments)
        {
            userManager.Broadcast(null, receivers, translations.Get(translationKey, arguments));
        }
    }
}