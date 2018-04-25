using Rocket.API.I18N;
using Rocket.API.Player;

namespace Rocket.API.Chat
{
    /* Do not add stuff like colors, formatting, etc here.
     * Implementations can extend this interface and add additional features like colors, formatting, etc. */
    public interface IChatManager
    {
        /// <summary>
        /// Sends a message to the given player
        /// </summary>
        /// <param name="player">The receiver of the message</param>
        /// <param name="message">The message to send</param>
        /// <param name="bindings">The bindings for the message</param>
        void SendMessage(IOnlinePlayer player, string message, params object[] bindings);

        /// <summary>
        /// Sends a localized message to the given player
        /// </summary>
        /// <param name="translationSource">The translation source</param>
        /// <param name="player">The receiver of the message</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="bindings">The bindings for the message</param>
        void SendLocalizedMessage(ITranslationLocator translationSource, IOnlinePlayer player, string translationKey,
                                  params object[] bindings);

        /// <summary>
        /// Broadcasts a message to all players
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        /// <param name="bindings">The bindings for the message</param> 
        void Broadcast(string message, params object[] bindings);

        /// <summary>
        /// Broadcasts a localized message to all players
        /// </summary>
        /// <param name="translations">The translation soruce</param>
        /// <param name="translationKey">The key of the translated message to send</param>
        /// <param name="bindings">The bindings for the message</param> 
        void BroadcastLocalized(ITranslationLocator translations, string translationKey, params object[] bindings);
    }
}