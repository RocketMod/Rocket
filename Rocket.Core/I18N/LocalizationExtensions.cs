using System;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;

namespace Rocket.Core.I18N
{
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Sends a localized (translatable) message to the command caller.
        /// </summary>
        /// <param name="commandCaller">The message receiver.</param>
        /// <param name="translations">The translations source.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="color">The color of the message. Depending on the caller implementation, it may not be used.</param>
        /// <param name="bindings">The bindings for the message. See <see cref="string.Format(string, object[])"/></param>
        public static void SendLocalizedMessage(this ICommandCaller commandCaller, ITranslationLocator translations, string translationKey,
                                                ConsoleColor? color = null, params object[] bindings)
        {
            commandCaller.SendMessage(translations.GetLocalizedMessage(translationKey, bindings), color);
        }

        /// <summary>
        /// Sends a localized message to the given player
        /// </summary>
        /// <param name="chatManager">The chat manager.</param>
        /// <param name="translationSource">The translation source</param>
        /// <param name="player">The receiver of the message</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="bindings">The bindings for the message</param>
        public static void SendLocalizedMessage(this IChatManager chatManager, ITranslationLocator translations, 
                                                IOnlinePlayer player, string translationKey, params object[] bindings)
        {
            chatManager.SendMessage(player, translations.GetLocalizedMessage(translationKey, bindings));
        }



        /// <summary>
        /// Broadcasts a localized message to all players
        /// </summary>
        /// <param name="chatManager">The chat manager.</param>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="bindings">The bindings for the message</param> 
        public static void BroadcastLocalized(this IChatManager chatManager, ITranslationLocator translations,
                                              string translationKey, params object[] bindings)
        {
            chatManager.Broadcast(translations.GetLocalizedMessage(translationKey, bindings));
        }
    }
}