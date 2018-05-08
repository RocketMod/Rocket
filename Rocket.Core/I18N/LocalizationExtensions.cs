using System;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;

namespace Rocket.Core.I18N
{
    public static class LocalizationExtensions
    {
        /// <summary>
        ///     Sends a localized (translatable) message to the command caller.
        /// </summary>
        /// <param name="User">The message receiver.</param>
        /// <param name="translations">The translations source.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" /></param>
        public static void SendLocalizedMessage(this IUser User, ITranslationLocator translations, string translationKey,params object[] arguments)
        {
            User.SendMessage(translations.GetLocalizedMessage(translationKey, arguments), color);
        }

        /// <summary>
        ///     Sends a localized message to the given player
        /// </summary>
        /// <param name="chatManager">The chat manager.</param>
        /// <param name="translations">The translation source</param>
        /// <param name="player">The receiver of the message</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="arguments">The arguments for the message</param>
        public static void SendLocalizedMessage(this IChatManager chatManager, ITranslationLocator translations, IOnlinePlayer player, string translationKey, params object[] arguments)
        {
            chatManager.SendMessage(player, translations.GetLocalizedMessage(translationKey, arguments));
        }

        /// <summary>
        ///     Broadcasts a localized message to all players
        /// </summary>
        /// <param name="chatManager">The chat manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="arguments">The arguments for the message</param>
        public static void BroadcastLocalized(this IChatManager chatManager, ITranslationLocator translations, string translationKey, params object[] arguments)
        {
            chatManager.Broadcast(translations.GetLocalizedMessage(translationKey, arguments));
        }
    }
}