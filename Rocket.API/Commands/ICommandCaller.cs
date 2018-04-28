using System;
using Rocket.API.Permissions;

namespace Rocket.API.Commands
{
    /// <summary>
    /// A command caller can execute commands.
    /// </summary>
    public interface ICommandCaller : IPermissible
    {
        /// <summary>
        /// The name of the command caller.<br/><br/>
        /// <b>This property will never return null.</b>
        /// </summary>
        string Name { get; }

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
        void SendMessage(string message, ConsoleColor? color = null);
    }
}