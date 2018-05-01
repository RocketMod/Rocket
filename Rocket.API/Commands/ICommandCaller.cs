using System;
using Rocket.API.Permissions;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     A command caller can execute commands.
    /// </summary>
    public interface ICommandCaller : IIdentifiable
    {
        /// <summary>
        ///     <para>The type of the command caller.</para> 
        ///     <para>Inheriting classes should declare their parent, everything else should declare itself.</para>
        ///     <para><b>This property will never return null.</b></para>
        /// </summary>
        Type CallerType { get; }

        /// <summary>
        ///     Send a message to the command caller.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color of the message. Depending on the command caller implementation, it may not be used.</param>
        /// <param name="bindings">The bindings for the message. See <see cref="string.Format(string, object[])" /></param>
        void SendMessage(string message, ConsoleColor? color = null, params object[] bindings);
    }
}