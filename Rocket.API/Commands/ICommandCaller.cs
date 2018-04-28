using System;
using Rocket.API.I18N;
using Rocket.API.Permissions;

namespace Rocket.API.Commands
{
    /// <summary>
    /// A command caller can execute commands.
    /// </summary>
    public interface ICommandCaller : IPermissible
    {
        /// <summary>
        /// The type of the command caller. Wrappers should declare their parents, everything else should declare itself.<br/><br/>
        /// <b>This property will never return null.</b>
        /// </summary>
        Type CallerType { get; }

        /// <summary>
        /// Send a message to the command caller.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color of the message. Depending on the caller implementation, it may not be used.</param>
        /// <param name="bindings">The bindings for the message. See <see cref="string.Format(string, object[])"/></param>
        void SendMessage(string message, ConsoleColor? color = null, params object[] bindings);

        /// <summary>
        /// Sends a localized (translatable) message to the command caller.
        /// </summary>
        /// <param name="translations">The translations source.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="color">The color of the message. Depending on the caller implementation, it may not be used.</param>
        /// <param name="bindings">The bindings for the message. See <see cref="string.Format(string, object[])"/></param>
        void SendLocalizedMessage(ITranslationLocator translations, string translationKey, ConsoleColor? color = null, params object[] bindings);
    }
}